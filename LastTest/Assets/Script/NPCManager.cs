using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour
{
    // NPC 성격을 Enum으로 정의
    public enum NPCPersonality
    {
        Optimistic,
        Pessimistic,
        Curious
    }

    // NPC 정보를 포함하는 구조체
    [System.Serializable]
    public class NPCInfo
    {
        public string npcName;  // NPC 이름
        public NPCPersonality personality;  // NPC 성격
        public TextMeshProUGUI npcStatusText;  // 상태를 표시할 텍스트
        public int coins = 10;  // NPC가 가진 코인
        public int chosenLocation;  // 선택한 장소
        public int betAmount;  // 투자한 코인 수치
        public GameObject npcMarkerPrefab; // NPC별 마커 프리팹

        [Header("문항별로 필요한 지역 설정")]
        public List<int> question1Locations = new List<int>(); // 1번 문항의 정답 지역
        public List<int> question2Locations = new List<int>(); // 2번 문항의 정답 지역
        public List<int> question3Locations = new List<int>(); // 3번 문항의 정답 지역

        [HideInInspector] public List<int> visitedLocations = new List<int>(); // 방문한 지역 기록

        [Header("정답 맞춤 여부")]
        public bool question1Solved = false;
        public bool question2Solved = false;
        public bool question3Solved = false;
    }

    public AnswerManager answerManager; // AnswerManager 스크립트 참조

    // 건물 위치 관리
    public List<Transform> buildingLocations; // 건물 위치 리스트

    public List<NPCInfo> npcList = new List<NPCInfo>();

    public Button nextTurnButton; // Next Turn 버튼을 인스펙터에서 설정

    private List<GameObject> activeMarkers = new List<GameObject>(); // 현재 활성화된 마커

    private int currentTurn = 1;
    private bool hasFoundAnswer = false;

    private const int LOCATION_COUNT = 8;

    void Start()
    {
        currentTurn = 1;

        // NPC 초기화
        foreach (var npc in npcList)
        {
            npc.npcStatusText.text = $"{npc.npcName}: Investigating...";
            npc.visitedLocations.Clear(); // 방문 지역 초기화
            npc.question1Solved = npc.question2Solved = npc.question3Solved = false;

            // PlayerPrefs에서 방문 기록 로드
            LoadVisitedLocations(npc);

            // NPC가 랜덤으로 장소를 선택하도록 설정 (이미 방문한 장소는 제외)
            npc.chosenLocation = GetRandomLocation(npc);
            npc.betAmount = npc.coins > 0 ? Random.Range(1, npc.coins + 1) : 0;
            npc.coins -= npc.betAmount;
            npc.visitedLocations.Add(npc.chosenLocation); // 방문한 장소 추가

            // NPC의 마커 위치 업데이트
            UpdateNpcMarker(npc, npc.chosenLocation);
        }

        // NPC 상태 갱신
        UpdateNpcStatuses();

        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.AddListener(NextTurn);
        }

        LoadNpcData(); // 이전 데이터 복원
    }

    public void NextTurn()
    {
        if (hasFoundAnswer) return;

        foreach (var npc in npcList)
        {
            // NPC가 랜덤으로 장소를 선택하도록 설정 (이미 방문한 장소는 제외)
            npc.chosenLocation = GetRandomLocation(npc);
            npc.betAmount = npc.coins > 0 ? Random.Range(1, npc.coins + 1) : 0;
            npc.coins -= npc.betAmount;
            npc.visitedLocations.Add(npc.chosenLocation); // 방문한 장소 기록

            // NPC의 마커 위치 업데이트
            UpdateNpcMarker(npc, npc.chosenLocation);
        }

        CheckQuestions(); // 문항 조건 확인

        currentTurn++;
        UpdateNpcStatuses();

        SaveNpcData(); // 데이터 저장
        SaveVisitedLocations(); // 방문한 장소 저장
    }

    int GetRandomLocation(NPCInfo npc)
    {
        List<int> availableLocations = new List<int>();

        // 방문하지 않은 장소만 선택지에 추가
        for (int i = 0; i < LOCATION_COUNT; i++)
        {
            if (!npc.visitedLocations.Contains(i) && !IsLocationAlreadyChosenByOtherNpc(i))
            {
                availableLocations.Add(i);
            }
        }

        // 방문하지 않은 장소가 없으면 모든 장소 중에서 랜덤 선택
        if (availableLocations.Count == 0)
        {
            availableLocations.AddRange(System.Linq.Enumerable.Range(0, LOCATION_COUNT));
        }

        // 랜덤으로 장소 선택
        return availableLocations[Random.Range(0, availableLocations.Count)];
    }

    bool IsLocationAlreadyChosenByOtherNpc(int location)
    {
        // 다른 NPC들이 해당 장소를 선택했는지 확인
        foreach (var npc in npcList)
        {
            if (npc.chosenLocation == location)
            {
                return true;  // 이미 다른 NPC가 해당 장소를 선택한 경우
            }
        }
        return false;  // 다른 NPC들이 선택하지 않은 경우
    }

    void CheckQuestions()
    {
        foreach (var npc in npcList)
        {
            // 모든 문항이 맞춰졌는지 확인
            if (npc.question1Solved && npc.question2Solved && npc.question3Solved)
            {
                Debug.Log($"{npc.npcName}이(가) 모든 문항을 맞췄습니다!");

                // AnswerManager의 isAnswerNPC를 true로 설정하고 CheckAnswers 호출
                if (answerManager != null)
                {
                    answerManager.isAnswerNPC = true;  // isAnswerNPC를 true로 설정
                    answerManager.CheckAnswers();  // CheckAnswers 함수 호출
                }
            }
            else
            {
                if (!npc.question1Solved && AllLocationsVisited(npc.visitedLocations, npc.question1Locations))
                {
                    npc.question1Solved = true;
                    Debug.Log($"{npc.npcName}이(가) 1번 문항을 맞췄습니다!");
                }

                if (!npc.question2Solved && AllLocationsVisited(npc.visitedLocations, npc.question2Locations))
                {
                    npc.question2Solved = true;
                    Debug.Log($"{npc.npcName}이(가) 2번 문항을 맞췄습니다!");
                }

                if (!npc.question3Solved && AllLocationsVisited(npc.visitedLocations, npc.question3Locations))
                {
                    npc.question3Solved = true;
                    Debug.Log($"{npc.npcName}이(가) 3번 문항을 맞췄습니다!");
                }
            }
        }
    }

    bool AllLocationsVisited(List<int> visitedLocations, List<int> requiredLocations)
    {
        foreach (int location in requiredLocations)
        {
            if (!visitedLocations.Contains(location))
                return false;
        }
        return true;
    }

    void UpdateNpcMarker(NPCInfo npc, int locationNumber)
    {
        Transform buildingTransform = GetBuildingLocation(locationNumber);
        if (buildingTransform == null)
        {
            Debug.LogWarning($"장소 번호 {locationNumber}에 해당하는 건물이 없습니다.");
            return;
        }

        // 마커가 위치할 기본 위치
        Vector3 baseMarkerPosition = buildingTransform.position + new Vector3(0, 1, 0);

        // 이미 해당 위치에 있는 마커들을 찾음
        List<GameObject> markersAtLocation = activeMarkers.FindAll(m => m.transform.parent == buildingTransform);

        // 해당 마커가 이미 존재하는지 확인
        GameObject marker = activeMarkers.Find(m => m.name == npc.npcName);
        if (marker == null)
        {
            // 새 마커 생성
            if (npc.npcMarkerPrefab == null)
            {
                Debug.LogError($"{npc.npcName}의 npcMarkerPrefab이 설정되지 않았습니다!");
                return;
            }

            float zOffset = markersAtLocation.Count * 0.3f; // 기존 마커 수에 따른 Z축 오프셋
            Vector3 markerPosition = baseMarkerPosition + new Vector3(0, 0, zOffset);

            marker = Instantiate(npc.npcMarkerPrefab, markerPosition, Quaternion.identity, buildingTransform);
            marker.name = npc.npcName;
            activeMarkers.Add(marker);
            Debug.Log($"[Marker Created] {npc.npcName}의 마커가 {markerPosition} 위치에 생성되었습니다.");
        }
        else
        {
            // 기존 마커가 있으면 위치 이동
            float zOffset = markersAtLocation.IndexOf(marker) * 0.3f; // 마커 인덱스에 따른 Z축 오프셋
            Vector3 markerPosition = baseMarkerPosition + new Vector3(0, 0, zOffset);

            marker.transform.position = markerPosition;
            Debug.Log($"[Marker Moved] {npc.npcName}의 마커가 {markerPosition} 위치로 이동했습니다.");
        }
    }

    Transform GetBuildingLocation(int locationNumber)
    {
        if (locationNumber >= 0 && locationNumber < buildingLocations.Count)
        {
            return buildingLocations[locationNumber];
        }
        Debug.LogWarning($"유효하지 않은 위치 번호입니다: {locationNumber}");
        return null;
    }

    void UpdateNpcStatuses()
    {
        foreach (var npc in npcList)
        {
            string statusMessage = "";

            if (npc.question1Solved && npc.question2Solved && npc.question3Solved)
            {
                statusMessage = $"{npc.npcName}: All Clear!";
            }
            else if (npc.question1Solved)
            {
                statusMessage = $"{npc.npcName}: One Clear!";
            }
            else if (npc.question2Solved)
            {
                statusMessage = $"{npc.npcName}: Two Clear!!";
            }
            else if (npc.question3Solved)
            {
                statusMessage = $"{npc.npcName}: Three Clear!!";
            }
            else
            {
                switch (npc.personality)
                {
                    case NPCPersonality.Optimistic:
                        statusMessage = $"{npc.npcName}: Hopeful as always!";
                        break;
                    case NPCPersonality.Pessimistic:
                        statusMessage = $"{npc.npcName}: This might not work...";
                        break;
                    case NPCPersonality.Curious:
                        statusMessage = $"{npc.npcName}: Let’s see where this leads!";
                        break;
                }
            }

            if (npc.npcStatusText != null)
            {
                npc.npcStatusText.text = statusMessage;
            }
        }
    }

    void SaveNpcData()
    {
        foreach (var npc in npcList)
        {
            // 모든 값을 포함하도록 확실히 저장
            string json = JsonUtility.ToJson(new SerializableNPC(npc));
            PlayerPrefs.SetString($"NPC_{npc.npcName}", json);
        }
        PlayerPrefs.Save();
    }


    void LoadNpcData()
    {
        foreach (var npc in npcList)
        {
            string key = $"NPC_{npc.npcName}";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key);
                SerializableNPC loadedData = JsonUtility.FromJson<SerializableNPC>(json);
                loadedData.ApplyTo(npc); // 저장된 값으로 덮어쓰기
            }
        }
    }


    // 방문한 장소를 PlayerPrefs에 저장
    void SaveVisitedLocations()
    {
        foreach (var npc in npcList)
        {
            string visitedLocationsKey = $"{npc.npcName}_visitedLocations";
            string json = JsonUtility.ToJson(npc.visitedLocations);
            PlayerPrefs.SetString(visitedLocationsKey, json);
        }
        PlayerPrefs.Save();
    }

    // PlayerPrefs에서 방문한 장소 로드
    void LoadVisitedLocations(NPCInfo npc)
    {
        string visitedLocationsKey = $"{npc.npcName}_visitedLocations";
        if (PlayerPrefs.HasKey(visitedLocationsKey))
        {
            string json = PlayerPrefs.GetString(visitedLocationsKey);
            npc.visitedLocations = JsonUtility.FromJson<List<int>>(json);
        }
    }
}
