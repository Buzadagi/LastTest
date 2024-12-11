using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    // Start ��ư�� Exit ��ư�� �ν����Ϳ��� ����
    public Button startButton;
    public Button exitButton;

    private void Start()
    {
        // Start ��ư Ŭ�� �̺�Ʈ ����
        startButton.onClick.AddListener(OnStartButtonClicked);

        // Exit ��ư Ŭ�� �̺�Ʈ ����
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene("Story"); // Story ������ �̵�
    }

    private void OnExitButtonClicked()
    {
        Application.Quit(); // ���� ����
        Debug.Log("Game is exiting..."); // Unity �����Ϳ��� Ȯ�ο� �α�
    }
}
