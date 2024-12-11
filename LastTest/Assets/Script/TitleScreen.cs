using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    // Start 버튼과 Exit 버튼을 인스펙터에서 연결
    public Button startButton;
    public Button exitButton;

    private void Start()
    {
        // Start 버튼 클릭 이벤트 연결
        startButton.onClick.AddListener(OnStartButtonClicked);

        // Exit 버튼 클릭 이벤트 연결
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Story"); // Story 씬으로 이동
    }

    private void OnExitButtonClicked()
    {
        Application.Quit(); // 게임 종료
        Debug.Log("Game is exiting..."); // Unity 에디터에서 확인용 로그
    }
}
