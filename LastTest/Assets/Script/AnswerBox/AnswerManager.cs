using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

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

    // isAnswerNPC 변수 추가
    public bool isAnswerNPC = false;

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

    private void Update()
    {
        // Tab 키로 TMP_InputField 이동
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable current = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();
            if (current != null)
            {
                Selectable next;

                // Shift+Tab: 위로 이동, Tab: 아래로 이동
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    next = current.FindSelectableOnUp();
                }
                else
                {
                    next = current.FindSelectableOnDown();

                    // 마지막 입력 필드에서 Tab을 누르면 첫 번째 입력 필드로 이동
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
        answer1Field.Select(); // UI가 활성화되면 첫 번째 입력 필드 선택
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

        // isAnswerNPC가 true일 때는 빈 문항 체크를 건너뜁니다.
        if (!isAnswerNPC && (string.IsNullOrEmpty(answer1) || string.IsNullOrEmpty(answer2) || string.IsNullOrEmpty(answer3)))
        {
            Debug.Log("빈 문항이 있습니다");
            return;
        }

        bool isAnswer1Correct = answer1 == "bequest";
        bool isAnswer2Correct = answer2 == "piano string";
        bool isAnswer3Correct = answer3 == "edward";

        // isAnswerNPC가 true일 경우 엔딩 번호를 4로 설정
        if (isAnswerNPC)
        {
            Debug.Log("NPC 엔딩");
            endingNumber = 4;
        }
        else if (!isAnswer3Correct)
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
