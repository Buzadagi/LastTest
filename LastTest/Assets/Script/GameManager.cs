using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // UI 요소
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI policeLineText;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI turnText; // Turn UI 추가

    // 초기 값
    public int defaultCoins = 10;
    public int defaultPoliceLines = 1;
    public int defaultKeys = 1;
    public int defaultTurns
        = 1; // 기본 Turn 값

    private void Start()
    {
        InitializeValues();
        UpdateUI();
    }

    private void InitializeValues()
    {
        if (!PlayerPrefs.HasKey("Coins"))
        {
            PlayerPrefs.SetInt("Coins", defaultCoins);
        }
        if (!PlayerPrefs.HasKey("PoliceLines"))
        {
            PlayerPrefs.SetInt("PoliceLines", defaultPoliceLines);
        }
        if (!PlayerPrefs.HasKey("Keys"))
        {
            PlayerPrefs.SetInt("Keys", defaultKeys);
        }
        if (!PlayerPrefs.HasKey("Turns")) // Turn 값 초기화
        {
            PlayerPrefs.SetInt("Turns", defaultTurns);
        }
    }

    private void UpdateUI()
    {
        coinText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
        policeLineText.text = "Police: " + PlayerPrefs.GetInt("PoliceLines");
        keyText.text = "Keys: " + PlayerPrefs.GetInt("Keys");
        turnText.text = "Turns: " + PlayerPrefs.GetInt("Turns"); // Turn UI 업데이트
    }

    public int GetPlayerCoins()
    {
        return PlayerPrefs.GetInt("Coins");
    }

    public void AddCoins(int amount)
    {
        int currentCoins = PlayerPrefs.GetInt("Coins");
        PlayerPrefs.SetInt("Coins", currentCoins + amount);
        UpdateUI();
    }

    public int GetPlayerKeys()
    {
        return PlayerPrefs.GetInt("Keys");
    }

    public void AddKeys(int amount)
    {
        int currentKeys = PlayerPrefs.GetInt("Keys");
        PlayerPrefs.SetInt("Keys", currentKeys + amount);
        UpdateUI();
    }

    public int GetTurns()
    {
        return PlayerPrefs.GetInt("Turns");
    }

    public void AddTurns(int amount)
    {
        int currentTurns = PlayerPrefs.GetInt("Turns");
        PlayerPrefs.SetInt("Turns", currentTurns + amount);
        UpdateUI();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
