using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPCManager : MonoBehaviour
{
    // NPC ������ Enum���� ����
    public enum NPCPersonality
    {
        Optimistic,
        Pessimistic,
        Curious
    }

    // NPC ������ �����ϴ� ����ü
    [System.Serializable]
    public class NPCInfo
    {
        public string npcName;  // NPC �̸�
        public NPCPersonality personality;  // NPC ����
        public TextMeshProUGUI npcStatusText;  // ���¸� ǥ���� �ؽ�Ʈ
        public int coins = 10;  // NPC�� ���� ����
        public int chosenLocation;  // ������ ���
        public int betAmount;  // ������ ���� ��ġ
        public GameObject npcMarkerPrefab; // NPC�� ��Ŀ ������

        [Header("���׺��� �ʿ��� ���� ����")]
        public List<int> question1Locations = new List<int>(); // 1�� ������ ���� ����
        public List<int> question2Locations = new List<int>(); // 2�� ������ ���� ����
        public List<int> question3Locations = new List<int>(); // 3�� ������ ���� ����

        [HideInInspector] public List<int> visitedLocations = new List<int>(); // �湮�� ���� ���

        [Header("���� ���� ����")]
        public bool question1Solved = false;
        public bool question2Solved = false;
        public bool question3Solved = false;
    }

    public AnswerManager answerManager; // AnswerManager ��ũ��Ʈ ����

    // �ǹ� ��ġ ����
    public List<Transform> buildingLocations; // �ǹ� ��ġ ����Ʈ

    public List<NPCInfo> npcList = new List<NPCInfo>();

    public Button nextTurnButton; // Next Turn ��ư�� �ν����Ϳ��� ����

    private List<GameObject> activeMarkers = new List<GameObject>(); // ���� Ȱ��ȭ�� ��Ŀ

    private int currentTurn = 1;
    private bool hasFoundAnswer = false;

    private const int LOCATION_COUNT = 8;

    void Start()
    {
        currentTurn = 1;

        // NPC �ʱ�ȭ
        foreach (var npc in npcList)
        {
            npc.npcStatusText.text = $"{npc.npcName}: Investigating...";
            npc.visitedLocations.Clear(); // �湮 ���� �ʱ�ȭ
            npc.question1Solved = npc.question2Solved = npc.question3Solved = false;

            // PlayerPrefs���� �湮 ��� �ε�
            LoadVisitedLocations(npc);

            // NPC�� �������� ��Ҹ� �����ϵ��� ���� (�̹� �湮�� ��Ҵ� ����)
            npc.chosenLocation = GetRandomLocation(npc);
            npc.betAmount = npc.coins > 0 ? Random.Range(1, npc.coins + 1) : 0;
            npc.coins -= npc.betAmount;
            npc.visitedLocations.Add(npc.chosenLocation); // �湮�� ��� �߰�

            // NPC�� ��Ŀ ��ġ ������Ʈ
            UpdateNpcMarker(npc, npc.chosenLocation);
        }

        // NPC ���� ����
        UpdateNpcStatuses();

        if (nextTurnButton != null)
        {
            nextTurnButton.onClick.AddListener(NextTurn);
        }

        LoadNpcData(); // ���� ������ ����
    }

    public void NextTurn()
    {
        if (hasFoundAnswer) return;

        foreach (var npc in npcList)
        {
            // NPC�� �������� ��Ҹ� �����ϵ��� ���� (�̹� �湮�� ��Ҵ� ����)
            npc.chosenLocation = GetRandomLocation(npc);
            npc.betAmount = npc.coins > 0 ? Random.Range(1, npc.coins + 1) : 0;
            npc.coins -= npc.betAmount;
            npc.visitedLocations.Add(npc.chosenLocation); // �湮�� ��� ���

            // NPC�� ��Ŀ ��ġ ������Ʈ
            UpdateNpcMarker(npc, npc.chosenLocation);
        }

        CheckQuestions(); // ���� ���� Ȯ��

        currentTurn++;
        UpdateNpcStatuses();

        SaveNpcData(); // ������ ����
        SaveVisitedLocations(); // �湮�� ��� ����
    }

    int GetRandomLocation(NPCInfo npc)
    {
        List<int> availableLocations = new List<int>();

        // �湮���� ���� ��Ҹ� �������� �߰�
        for (int i = 0; i < LOCATION_COUNT; i++)
        {
            if (!npc.visitedLocations.Contains(i) && !IsLocationAlreadyChosenByOtherNpc(i))
            {
                availableLocations.Add(i);
            }
        }

        // �湮���� ���� ��Ұ� ������ ��� ��� �߿��� ���� ����
        if (availableLocations.Count == 0)
        {
            availableLocations.AddRange(System.Linq.Enumerable.Range(0, LOCATION_COUNT));
        }

        // �������� ��� ����
        return availableLocations[Random.Range(0, availableLocations.Count)];
    }

    bool IsLocationAlreadyChosenByOtherNpc(int location)
    {
        // �ٸ� NPC���� �ش� ��Ҹ� �����ߴ��� Ȯ��
        foreach (var npc in npcList)
        {
            if (npc.chosenLocation == location)
            {
                return true;  // �̹� �ٸ� NPC�� �ش� ��Ҹ� ������ ���
            }
        }
        return false;  // �ٸ� NPC���� �������� ���� ���
    }

    void CheckQuestions()
    {
        foreach (var npc in npcList)
        {
            // ��� ������ ���������� Ȯ��
            if (npc.question1Solved && npc.question2Solved && npc.question3Solved)
            {
                Debug.Log($"{npc.npcName}��(��) ��� ������ ������ϴ�!");

                // AnswerManager�� isAnswerNPC�� true�� �����ϰ� CheckAnswers ȣ��
                if (answerManager != null)
                {
                    answerManager.isAnswerNPC = true;  // isAnswerNPC�� true�� ����
                    answerManager.CheckAnswers();  // CheckAnswers �Լ� ȣ��
                }
            }
            else
            {
                if (!npc.question1Solved && AllLocationsVisited(npc.visitedLocations, npc.question1Locations))
                {
                    npc.question1Solved = true;
                    Debug.Log($"{npc.npcName}��(��) 1�� ������ ������ϴ�!");
                }

                if (!npc.question2Solved && AllLocationsVisited(npc.visitedLocations, npc.question2Locations))
                {
                    npc.question2Solved = true;
                    Debug.Log($"{npc.npcName}��(��) 2�� ������ ������ϴ�!");
                }

                if (!npc.question3Solved && AllLocationsVisited(npc.visitedLocations, npc.question3Locations))
                {
                    npc.question3Solved = true;
                    Debug.Log($"{npc.npcName}��(��) 3�� ������ ������ϴ�!");
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
            Debug.LogWarning($"��� ��ȣ {locationNumber}�� �ش��ϴ� �ǹ��� �����ϴ�.");
            return;
        }

        // ��Ŀ�� ��ġ�� �⺻ ��ġ
        Vector3 baseMarkerPosition = buildingTransform.position + new Vector3(0, 1, 0);

        // �̹� �ش� ��ġ�� �ִ� ��Ŀ���� ã��
        List<GameObject> markersAtLocation = activeMarkers.FindAll(m => m.transform.parent == buildingTransform);

        // �ش� ��Ŀ�� �̹� �����ϴ��� Ȯ��
        GameObject marker = activeMarkers.Find(m => m.name == npc.npcName);
        if (marker == null)
        {
            // �� ��Ŀ ����
            if (npc.npcMarkerPrefab == null)
            {
                Debug.LogError($"{npc.npcName}�� npcMarkerPrefab�� �������� �ʾҽ��ϴ�!");
                return;
            }

            float zOffset = markersAtLocation.Count * 0.3f; // ���� ��Ŀ ���� ���� Z�� ������
            Vector3 markerPosition = baseMarkerPosition + new Vector3(0, 0, zOffset);

            marker = Instantiate(npc.npcMarkerPrefab, markerPosition, Quaternion.identity, buildingTransform);
            marker.name = npc.npcName;
            activeMarkers.Add(marker);
            Debug.Log($"[Marker Created] {npc.npcName}�� ��Ŀ�� {markerPosition} ��ġ�� �����Ǿ����ϴ�.");
        }
        else
        {
            // ���� ��Ŀ�� ������ ��ġ �̵�
            float zOffset = markersAtLocation.IndexOf(marker) * 0.3f; // ��Ŀ �ε����� ���� Z�� ������
            Vector3 markerPosition = baseMarkerPosition + new Vector3(0, 0, zOffset);

            marker.transform.position = markerPosition;
            Debug.Log($"[Marker Moved] {npc.npcName}�� ��Ŀ�� {markerPosition} ��ġ�� �̵��߽��ϴ�.");
        }
    }

    Transform GetBuildingLocation(int locationNumber)
    {
        if (locationNumber >= 0 && locationNumber < buildingLocations.Count)
        {
            return buildingLocations[locationNumber];
        }
        Debug.LogWarning($"��ȿ���� ���� ��ġ ��ȣ�Դϴ�: {locationNumber}");
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
                        statusMessage = $"{npc.npcName}: Let��s see where this leads!";
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
            // ��� ���� �����ϵ��� Ȯ���� ����
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
                loadedData.ApplyTo(npc); // ����� ������ �����
            }
        }
    }


    // �湮�� ��Ҹ� PlayerPrefs�� ����
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

    // PlayerPrefs���� �湮�� ��� �ε�
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
