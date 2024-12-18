using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager Instance;
    public GameObject answerUI; // ���� ĵ����
    public TMP_InputField answer1Field;
    public TMP_InputField answer2Field;
    public TMP_InputField answer3Field;
    public Button confirmButton; // Ȯ�� ��ư
    public Button exitButton; // Exit ��ư �߰�
    private int endingNumber = 0; // ���� ��ȣ ����

    // isAnswerNPC ���� �߰�
    public bool isAnswerNPC = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        answerUI.SetActive(false);

        // Ȯ�� ��ư�� �̺�Ʈ �߰�
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(CheckAnswers);
        }

        // Exit ��ư�� �̺�Ʈ �߰�
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(HideAnswerUI); // ��ư Ŭ�� �� UI ��Ȱ��ȭ
        }
    }

    private void Update()
    {
        // Tab Ű�� TMP_InputField �̵�
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
            if (current != null)
            {
                Selectable next;

                // Shift+Tab: ���� �̵�, Tab: �Ʒ��� �̵�
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    next = current.FindSelectableOnUp();
                }
                else
                {
                    next = current.FindSelectableOnDown();

                    // ������ �Է� �ʵ忡�� Tab�� ������ ù ��° �Է� �ʵ�� �̵�
                    if (next == null && current == answer3Field)
                    {
                        next = answer1Field;
                    }
                }

                if (next != null)
                {
                    EventSystem.current.SetSelectedGameObject(next.gameObject);
                }
            }
        }
    }

    public void ShowAnswerUI()
    {
        answerUI.SetActive(true);
        answer1Field.Select(); // UI�� Ȱ��ȭ�Ǹ� ù ��° �Է� �ʵ� ����
    }

    public void HideAnswerUI()
    {
        answerUI.SetActive(false);
    }

    public void CheckAnswers()
    {
        string answer1 = answer1Field.text.Trim().ToLower();
        string answer2 = answer2Field.text.Trim().ToLower();
        string answer3 = answer3Field.text.Trim().ToLower();

        // isAnswerNPC�� true�� ���� �� ���� üũ�� �ǳʶݴϴ�.
        if (!isAnswerNPC && (string.IsNullOrEmpty(answer1) || string.IsNullOrEmpty(answer2) || string.IsNullOrEmpty(answer3)))
        {
            Debug.Log("�� ������ �ֽ��ϴ�");
            return;
        }

        bool isAnswer1Correct = answer1 == "bequest";
        bool isAnswer2Correct = answer2 == "piano string";
        bool isAnswer3Correct = answer3 == "edward";

        // isAnswerNPC�� true�� ��� ���� ��ȣ�� 4�� ����
        if (isAnswerNPC)
        {
            Debug.Log("NPC ����");
            endingNumber = 4;
        }
        else if (!isAnswer3Correct)
        {
            Debug.Log("���� ����");
            endingNumber = 3;
        }
        else if (!isAnswer1Correct || !isAnswer2Correct)
        {
            Debug.Log("�븻 ����");
            endingNumber = 2;
        }
        else
        {
            Debug.Log("���� ����");
            endingNumber = 1;
        }

        // Story�� �̵�
        LoadEndingScene();
    }

    private void LoadEndingScene()
    {
        PlayerPrefs.SetInt("EndingNumber", endingNumber); // ���� ��ȣ�� ����
        PlayerPrefs.SetString("CSVFileName", "Ending.csv"); // ����� CSV ���� �̸� ����
        Debug.Log($"���� ��ȣ �����: {endingNumber}");
        SceneManager.LoadScene("Story"); // ���� ������ ��ȯ
    }
}
