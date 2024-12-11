using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour
{
    public bool isUIElement = false; // UI 요소인지 여부를 명시적으로 구분

    public static void LoadSceneWithPosition(string sceneName, Vector3 position)
    {
        PlayerPrefs.SetFloat("TargetX", position.x);
        PlayerPrefs.SetFloat("TargetY", position.y);
        PlayerPrefs.SetFloat("TargetZ", position.z);

        SceneManager.LoadScene(sceneName);
    }

    private void Start()
    {
        if (isUIElement) return; // UI 요소는 위치 변경을 건너뜀

        if (PlayerPrefs.HasKey("TargetX") && PlayerPrefs.HasKey("TargetY") && PlayerPrefs.HasKey("TargetZ"))
        {
            float x = PlayerPrefs.GetFloat("TargetX");
            float y = PlayerPrefs.GetFloat("TargetY");
            float z = PlayerPrefs.GetFloat("TargetZ");

            transform.position = new Vector3(x, y, z);

            PlayerPrefs.DeleteKey("TargetX");
            PlayerPrefs.DeleteKey("TargetY");
            PlayerPrefs.DeleteKey("TargetZ");
        }
    }
}
