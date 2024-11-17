using UnityEngine;
using TMPro; // Text Mesh Pro�� ����ϱ� ���� ���ӽ����̽�

public class GameManager : MonoBehaviour
{
    // UI ���
    public TextMeshProUGUI coinText;
    public TextMeshProUGUI policeLineText;
    public TextMeshProUGUI keyText;
    public TextMeshProUGUI turnText;

    // �ʱ� ��
    public int defaultCoins = 10;
    public int defaultPoliceLines = 1;
    public int defaultKeys = 1;
    public int defaultTurns = 1;

    private void Start()
    {
        // �� �ʱ�ȭ (ó�� ���� ��)
        InitializeValues();

        // UI ������Ʈ
        UpdateUI();
    }

    private void InitializeValues()
    {
        // PlayerPrefs�� ���� ������ �ʱ� �� ����
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
        // PlayerPrefs���� �� �о����
        coinText.text = "Coins: " + PlayerPrefs.GetInt("Coins");
        policeLineText.text = "Police: " + PlayerPrefs.GetInt("PoliceLines");
        keyText.text = "Keys: " + PlayerPrefs.GetInt("Keys");
        //turnText.text = "Turns: " + PlayerPrefs.GetInt("Turns");
    }

    // �� ���� �Լ�
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

    // ���� ���� �� ������ ����
    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }
}
