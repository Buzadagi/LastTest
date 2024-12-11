using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ObjectInteractionHandler : MonoBehaviour
{
    [Header("Object Information")]
    public string locationName; // ��� �̸�
    public string locationDescription; // ��� ����
    public bool isLocked = true; // �ʱ� ��� ����
    public string sceneName; // �̵��� �� �̸�
    public Vector3 destinationPosition; // �� �������� ��ǥ

    [Header("Global UI References")]
    public GameObject descriptionUI; // ���� �̹��� UI
    public TextMeshProUGUI nameText; // ��� �̸� �ؽ�Ʈ
    public TextMeshProUGUI descriptionText; // ��� ���� �ؽ�Ʈ
    public Button actionButton; // ���� ��ư �Ǵ� ��� ���� ��ư
    public GameObject unlockPromptUI; // ��� ���� UI
    public TextMeshProUGUI unlockPromptText; // ��� ���� �޽���
    public Button unlockYesButton; // ��� ���� �� ��ư
    public Button unlockNoButton; // ��� ���� �ƴϿ� ��ư
    public GameObject keyAlertUI; // ���� �ʿ� �ȳ� UI
    public TextMeshProUGUI keyAlertText; // ���� �ʿ� �ؽ�Ʈ
    public GameObject coinVestUI; // Coin Vest UI
    public TextMeshProUGUI countText; // Coin Vest�� Count Text
    public Button minusButton; // Coin Vest�� Minus Button
    public Button plusButton; // Coin Vest�� Plus Button
    public Button okButton; // Coin Vest�� OK Button
    public Button nextButton; // NextButton

    [Header("Icon References")]
    public GameObject coinCountIconPrefab; // CoinCount Icon Prefab 

    [Header("GameManager Reference")]
    private GameManager gameManager; // GameManager ����

    private static GameObject activeDescriptionUI; // ���� Ȱ��ȭ�� ���� UI
    private Coroutine fadeCoroutine;

    private int currentInvestment = 0; // Coin Vest���� ������ �ݾ�
    private int pendingInvestment = 0; // NextButton Ŭ�� ������ ������ �ӽ� �ݾ�

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (descriptionUI != null) descriptionUI.SetActive(false);
        if (unlockPromptUI != null) unlockPromptUI.SetActive(false);
        if (keyAlertUI != null) keyAlertUI.SetActive(false);
        if (coinVestUI != null) coinVestUI.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(false); // NextButton �ʱ�ȭ
    }

    private void OnMouseEnter()
    {
        if (CompareTag("Struct"))
        {
            HighlightObject(true); // �ܰ��� Ȱ��ȭ
        }
    }

    private void OnMouseExit()
    {
        if (CompareTag("Struct") && !IsMouseOverUI())
        {
            HighlightObject(false); // �ܰ��� ��Ȱ��ȭ
            CloseDescriptionUI(); // ���� UI �ݱ�
        }
    }

    private void OnMouseDown()
    {
        if (CompareTag("Struct"))
        {
            ShowDescriptionUI();
        }
    }

    private void ShowDescriptionUI()
    {
        if (activeDescriptionUI != null && activeDescriptionUI != descriptionUI)
        {
            activeDescriptionUI.SetActive(false);
        }

        activeDescriptionUI = descriptionUI;

        nameText.text = locationName;
        descriptionText.text = locationDescription;

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 adjustedPosition = new Vector3(screenPosition.x + 150, screenPosition.y + 100, screenPosition.z);
        descriptionUI.transform.position = adjustedPosition;

        descriptionUI.SetActive(true);

        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = isLocked ? "Lock State" : "Enter";
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(isLocked ? HandleLockedState : EnterLocation);
    }

    private void HandleLockedState()
    {
        int playerKeys = gameManager.GetPlayerKeys();

        if (playerKeys > 0)
        {
            unlockPromptUI.SetActive(true);
            unlockPromptText.text = "Do you want to use the key to unlock it?";
            unlockYesButton.onClick.RemoveAllListeners();
            unlockYesButton.onClick.AddListener(UnlockLocation);
            unlockNoButton.onClick.RemoveAllListeners();
            unlockNoButton.onClick.AddListener(CloseUnlockPromptUI);
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            keyAlertUI.SetActive(true);
            keyAlertText.text = "You need a key";
            fadeCoroutine = StartCoroutine(FadeOutKeyAlert());
        }
    }

    private void UnlockLocation()
    {
        gameManager.AddKeys(-1);
        isLocked = false;
        unlockPromptUI.SetActive(false);
        ShowDescriptionUI();
    }

    public void SaveLocationName()
    {
        PlayerPrefs.SetString("CurrentLocationName", locationName);
    }

    private void EnterLocation()
    {
        // locationName�� PlayerPrefs�� ����
        SaveLocationName();

        coinVestUI.SetActive(true);

        currentInvestment = 0;
        pendingInvestment = 0;
        countText.text = currentInvestment.ToString();

        int maxCoins = PlayerPrefs.GetInt("Coins");

        minusButton.onClick.RemoveAllListeners();
        minusButton.onClick.AddListener(() =>
        {
            if (currentInvestment > 0)
            {
                currentInvestment--;
                countText.text = currentInvestment.ToString();
            }
        });

        plusButton.onClick.RemoveAllListeners();
        plusButton.onClick.AddListener(() =>
        {
            if (currentInvestment < maxCoins)
            {
                currentInvestment++;
                countText.text = currentInvestment.ToString();
            }
        });

        okButton.onClick.RemoveAllListeners();
        okButton.onClick.AddListener(() =>
        {
            pendingInvestment = currentInvestment;

            // ���� �� ������Ʈ
            int currentCoins = PlayerPrefs.GetInt("Coins");
            PlayerPrefs.SetInt("Coins", currentCoins - pendingInvestment); // ������ ��ŭ ���� ����

            coinVestUI.SetActive(false);
            nextButton.gameObject.SetActive(true);

            Debug.Log($"Pending investment: {pendingInvestment} coins in {locationName}");
        });

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            // 'Turns' �� ����
            int currentTurns = gameManager.GetTurns();
            gameManager.AddTurns(1); // Turn �� 1 ����
            Debug.Log($"Turn increased to: {currentTurns + 1}");

            // �� �̵�
            SceneMove.LoadSceneWithPosition(sceneName, destinationPosition);
        });
    }

    private void CloseUnlockPromptUI()
    {
        unlockPromptUI.SetActive(false);
    }

    private IEnumerator FadeOutKeyAlert()
    {
        CanvasGroup canvasGroup = keyAlertUI.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1f);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }

        keyAlertUI.SetActive(false);
        canvasGroup.alpha = 1f;
    }

    private void CloseDescriptionUI()
    {
        if (descriptionUI != null)
        {
            descriptionUI.SetActive(false);
        }

        HighlightObject(false);
    }

    private void HighlightObject(bool highlight)
    {
        // �ܰ��� ǥ�� ����
    }

    private bool IsMouseOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
