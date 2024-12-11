using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class NPC : MonoBehaviour
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
        public NPCPersonality personality;  // NPC ���� (��Ӵٿ� �޴�)
        public TextMeshProUGUI npcStatusText;  // ���¸� ǥ���� �ؽ�Ʈ
    }

    // NPC ����Ʈ�� �ν����Ϳ��� ����
    public List<NPCInfo> npcList = new List<NPCInfo>();

    private GameManager gameManager;
    private int currentTurn = 1;  // ���� ��
    private string[] clues = { "Found the first clue", "Found the second clue", "Found the answer" };  // �ܼ���
    private bool hasFoundAnswer = false;  // ������ ã�Ҵ��� ����

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();  // GameManager ã��
        currentTurn = gameManager.GetTurns();  // �ʱ� �� �� ����

        // ��� NPC�� �ʱ� ���� ������Ʈ
        foreach (var npc in npcList)
        {
            npc.npcStatusText.text = $"{npc.npcName}: Investigating...";
        }

        UpdateNpcStatuses();
    }

    void UpdateNpcStatuses()
    {
        // �� NPC�� ���� ���¸� ������Ʈ
        foreach (var npc in npcList)
        {
            // NPC ���� ���
            if (hasFoundAnswer) return;  // �̹� ������ ã�� NPC�� ���� ������Ʈ �� ��

            currentTurn = gameManager.GetTurns();  // ���� �Ŵ������� ���� �� �� ��������

            // NPC ���ݿ� ���� �ؽ�Ʈ�� �ٸ��� ���
            string statusMessage = GetNpcStatusMessage(npc.npcName, currentTurn, npc.personality);
            if (npc.npcStatusText != null)
            {
                npc.npcStatusText.text = statusMessage;
            }
        }
    }

    // NPC �̸��� �Ͽ� ���� �ؽ�Ʈ �޽����� ����
    string GetNpcStatusMessage(string npcName, int turn, NPCPersonality personality)
    {
        string statusMessage = "";

        // ���ݿ� ���� �ٸ��� �ؽ�Ʈ�� ����
        switch (personality)
        {
            case NPCPersonality.Optimistic:
                switch (turn)
                {
                    case 1:
                        statusMessage = $"{npcName}: Everything will be fine, I��m sure I��ll find something!";
                        break;
                    case 2:
                        statusMessage = $"{npcName}: I think I found a clue! Things are looking up!";
                        break;
                    case 3:
                        statusMessage = $"{npcName}: I can feel it! The answer is so close!";
                        break;
                    default:
                        statusMessage = $"{npcName}: I��m almost there, the answer is in my grasp!";
                        break;
                }
                break;

            case NPCPersonality.Pessimistic:
                switch (turn)
                {
                    case 1:
                        statusMessage = $"{npcName}: I��m not sure if this is going anywhere...";
                        break;
                    case 2:
                        statusMessage = $"{npcName}: Another clue, but it doesn��t feel like enough. I doubt this will work.";
                        break;
                    case 3:
                        statusMessage = $"{npcName}: I��m not sure we��re even close to the answer...";
                        break;
                    default:
                        statusMessage = $"{npcName}: I don��t think we��ll ever find the answer. What a waste of time.";
                        break;
                }
                break;

            case NPCPersonality.Curious:
                switch (turn)
                {
                    case 1:
                        statusMessage = $"{npcName}: I��m excited! Let��s see what I can discover!";
                        break;
                    case 2:
                        statusMessage = $"{npcName}: This clue is fascinating! I need to know more!";
                        break;
                    case 3:
                        statusMessage = $"{npcName}: This is getting intense! I can feel the truth coming!";
                        break;
                    default:
                        statusMessage = $"{npcName}: I��m almost there, the answer is just around the corner!";
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
