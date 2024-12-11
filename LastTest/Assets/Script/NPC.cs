using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NPC : MonoBehaviour
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
        public NPCPersonality personality;  // NPC 성격 (드롭다운 메뉴)
        public TextMeshProUGUI npcStatusText;  // 상태를 표시할 텍스트
    }

    // NPC 리스트를 인스펙터에서 관리
    public List<NPCInfo> npcList = new List<NPCInfo>();

    private GameManager gameManager;
    private int currentTurn = 1;  // 현재 턴
    private string[] clues = { "Found the first clue", "Found the second clue", "Found the answer" };  // 단서들
    private bool hasFoundAnswer = false;  // 정답을 찾았는지 여부

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();  // GameManager 찾기
        currentTurn = gameManager.GetTurns();  // 초기 턴 값 설정

        // 모든 NPC의 초기 상태 업데이트
        foreach (var npc in npcList)
        {
            npc.npcStatusText.text = $"{npc.npcName}: Investigating...";
        }

        UpdateNpcStatuses();
    }

    void UpdateNpcStatuses()
    {
        // 각 NPC에 대해 상태를 업데이트
        foreach (var npc in npcList)
        {
            // NPC 상태 출력
            if (hasFoundAnswer) return;  // 이미 정답을 찾은 NPC는 상태 업데이트 안 함

            currentTurn = gameManager.GetTurns();  // 게임 매니저에서 현재 턴 값 가져오기

            // NPC 성격에 맞춰 텍스트를 다르게 출력
            string statusMessage = GetNpcStatusMessage(npc.npcName, currentTurn, npc.personality);
            if (npc.npcStatusText != null)
            {
                npc.npcStatusText.text = statusMessage;
            }
        }
    }

    // NPC 이름과 턴에 따른 텍스트 메시지를 생성
    string GetNpcStatusMessage(string npcName, int turn, NPCPersonality personality)
    {
        string statusMessage = "";

        // 성격에 따른 다르게 텍스트를 구성
        switch (personality)
        {
            case NPCPersonality.Optimistic:
                switch (turn)
                {
                    case 1:
                        statusMessage = $"{npcName}: Everything will be fine, I’m sure I’ll find something!";
                        break;
                    case 2:
                        statusMessage = $"{npcName}: I think I found a clue! Things are looking up!";
                        break;
                    case 3:
                        statusMessage = $"{npcName}: I can feel it! The answer is so close!";
                        break;
                    default:
                        statusMessage = $"{npcName}: I’m almost there, the answer is in my grasp!";
                        break;
                }
                break;

            case NPCPersonality.Pessimistic:
                switch (turn)
                {
                    case 1:
                        statusMessage = $"{npcName}: I’m not sure if this is going anywhere...";
                        break;
                    case 2:
                        statusMessage = $"{npcName}: Another clue, but it doesn’t feel like enough. I doubt this will work.";
                        break;
                    case 3:
                        statusMessage = $"{npcName}: I’m not sure we’re even close to the answer...";
                        break;
                    default:
                        statusMessage = $"{npcName}: I don’t think we’ll ever find the answer. What a waste of time.";
                        break;
                }
                break;

            case NPCPersonality.Curious:
                switch (turn)
                {
                    case 1:
                        statusMessage = $"{npcName}: I’m excited! Let’s see what I can discover!";
                        break;
                    case 2:
                        statusMessage = $"{npcName}: This clue is fascinating! I need to know more!";
                        break;
                    case 3:
                        statusMessage = $"{npcName}: This is getting intense! I can feel the truth coming!";
                        break;
                    default:
                        statusMessage = $"{npcName}: I’m almost there, the answer is just around the corner!";
                        break;
                }
                break;

            default:
                statusMessage = $"{npcName}: Investigating...";
                break;
        }

        return statusMessage;
    }
}
