using UnityEngine;
using TMPro; // Text Mesh Pro를 사용하기 위한 네임스페이스

public class GameManager : MonoBehaviour
{
    // UI 요소
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI policeLineText;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI turnText;

    // 초기 값
    public int defaultCoins = 10;
    public int defaultPoliceLines = 1;
    public int defaultKeys = 1;
    public int defaultTurns = 1;

    private void Start()
    {
        // 값 초기화 (처음 실행 시)
        InitializeValues();

        // UI 업데이트
        UpdateUI();
    }

    private void InitializeValues()
    {
        // PlayerPrefs에 값이 없으면 초기 값 설정
        if (!PlayerPrefs.HasKey("Coins"))
        {
            PlayerPrefs.SetInt("Coins", defaultCoins);
        }
        if (!PlayerPrefs.HasKey("Police"))
        {
            PlayerPrefs.SetInt("Police", defaultPoliceLines);
        }
        if (!PlayerPrefs.HasKey("Keys"))
        {
            PlayerPrefs.SetInt("Keys", defaultKeys);
        }
        if (!PlayerPrefs.HasKey("Turns"))
        {
            PlayerPrefs.SetInt("Turns", defaultTurns);
        }
    }

    private void UpdateUI()
    {
        // PlayerPrefs에서 값 읽어오기
        coinText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
        policeLineText.text = "Police: " + PlayerPrefs.GetInt("PoliceLines");
        keyText.text = "Keys: " + PlayerPrefs.GetInt("Keys");
        //turnText.text = "Turns: " + PlayerPrefs.GetInt("Turns");
    }

    // 값 변경 함수
    public void AddCoins(int amount)
    {
        int currentCoins = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", currentCoins + amount);
        UpdateUI();
    }

    public void AddPoliceLines(int amount)
    {
        int currentPoliceLines = PlayerPrefs.GetInt("PoliceLines");
        PlayerPrefs.SetInt("PoliceLines", currentPoliceLines + amount);
        UpdateUI();
    }

    public void AddKeys(int amount)
    {
        int currentKeys = PlayerPrefs.GetInt("Keys");
        PlayerPrefs.SetInt("Keys", currentKeys + amount);
        UpdateUI();
    }

    public void AddTurns(int amount)
    {
        int currentTurns = PlayerPrefs.GetInt("Turns");
        PlayerPrefs.SetInt("Turns", currentTurns + amount);
        UpdateUI();
    }

    // 게임 종료 시 데이터 저장
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
