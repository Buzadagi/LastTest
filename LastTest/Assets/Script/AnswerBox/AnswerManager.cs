using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    public void ShowAnswerUI()
    {
        answerUI.SetActive(true);
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

        if (string.IsNullOrEmpty(answer1) || string.IsNullOrEmpty(answer2) || string.IsNullOrEmpty(answer3))
        {
            Debug.Log("�� ������ �ֽ��ϴ�");
            return;
        }

        bool isAnswer1Correct = answer1 == "one";
        bool isAnswer2Correct = answer2 == "two";
        bool isAnswer3Correct = answer3 == "three";

        if (!isAnswer3Correct)
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
