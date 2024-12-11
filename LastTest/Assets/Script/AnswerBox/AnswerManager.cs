using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class AnswerManager : MonoBehaviour
{
    public static AnswerManager Instance;
    public GameObject answerUI; // 정답 캔버스
    public TMP_InputField answer1Field;
    public TMP_InputField answer2Field;
    public TMP_InputField answer3Field;
    public Button confirmButton; // 확인 버튼
    public Button exitButton; // Exit 버튼 추가
    private int endingNumber = 0; // 엔딩 번호 저장

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        answerUI.SetActive(false);

        // 확인 버튼에 이벤트 추가
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(CheckAnswers);
        }

        // Exit 버튼에 이벤트 추가
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(HideAnswerUI); // 버튼 클릭 시 UI 비활성화
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
            Debug.Log("빈 문항이 있습니다");
            return;
        }

        bool isAnswer1Correct = answer1 == "one";
        bool isAnswer2Correct = answer2 == "two";
        bool isAnswer3Correct = answer3 == "three";

        if (!isAnswer3Correct)
        {
            Debug.Log("새드 엔딩");
            endingNumber = 3;
        }
        else if (!isAnswer1Correct || !isAnswer2Correct)
        {
            Debug.Log("노말 엔딩");
            endingNumber = 2;
        }
        else
        {
            Debug.Log("해피 엔딩");
            endingNumber = 1;
        }

        // Story로 이동
        LoadEndingScene();
    }

    private void LoadEndingScene()
    {
        PlayerPrefs.SetInt("EndingNumber", endingNumber); // 엔딩 번호를 저장
        PlayerPrefs.SetString("CSVFileName", "Ending.csv"); // 사용할 CSV 파일 이름 저장
        Debug.Log($"엔딩 번호 저장됨: {endingNumber}");
        SceneManager.LoadScene("Story"); // 엔딩 씬으로 전환
    }
}
