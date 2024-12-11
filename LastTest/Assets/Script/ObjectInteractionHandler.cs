using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ObjectInteractionHandler : MonoBehaviour
{
    [Header("Object Information")]
    public string locationName; // 장소 이름
    public string locationDescription; // 장소 설명
    public bool isLocked = true; // 초기 잠금 상태
    public string sceneName; // 이동할 씬 이름
    public Vector3 destinationPosition; // 씬 내에서의 좌표

    [Header("Global UI References")]
    public GameObject descriptionUI; // 설명 이미지 UI
    public TextMeshProUGUI nameText; // 장소 이름 텍스트
    public TextMeshProUGUI descriptionText; // 장소 설명 텍스트
    public Button actionButton; // 입장 버튼 또는 잠금 상태 버튼
    public GameObject unlockPromptUI; // 잠금 해제 UI
    public TextMeshProUGUI unlockPromptText; // 잠금 해제 메시지
    public Button unlockYesButton; // 잠금 해제 예 버튼
    public Button unlockNoButton; // 잠금 해제 아니오 버튼
    public GameObject keyAlertUI; // 열쇠 필요 안내 UI
    public TextMeshProUGUI keyAlertText; // 열쇠 필요 텍스트
    public GameObject coinVestUI; // Coin Vest UI
    public TextMeshProUGUI countText; // Coin Vest의 Count Text
    public Button minusButton; // Coin Vest의 Minus Button
    public Button plusButton; // Coin Vest의 Plus Button
    public Button okButton; // Coin Vest의 OK Button
    public Button nextButton; // NextButton

    [Header("Icon References")]
    public GameObject coinCountIconPrefab; // CoinCount Icon Prefab 

    [Header("GameManager Reference")]
    private GameManager gameManager; // GameManager 참조

    private static GameObject activeDescriptionUI; // 현재 활성화된 설명 UI
    private Coroutine fadeCoroutine;

    private int currentInvestment = 0; // Coin Vest에서 설정한 금액
    private int pendingInvestment = 0; // NextButton 클릭 전까지 관리할 임시 금액

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        if (descriptionUI != null) descriptionUI.SetActive(false);
        if (unlockPromptUI != null) unlockPromptUI.SetActive(false);
        if (keyAlertUI != null) keyAlertUI.SetActive(false);
        if (coinVestUI != null) coinVestUI.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(false); // NextButton 초기화
    }

    private void OnMouseEnter()
    {
        if (CompareTag("Struct"))
        {
            HighlightObject(true); // 외곽선 활성화
        }
    }

    private void OnMouseExit()
    {
        if (CompareTag("Struct") && !IsMouseOverUI())
        {
            HighlightObject(false); // 외곽선 비활성화
            CloseDescriptionUI(); // 설명 UI 닫기
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
        // locationName을 PlayerPrefs에 저장
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

            // 코인 수 업데이트
            int currentCoins = PlayerPrefs.GetInt("Coins");
            PlayerPrefs.SetInt("Coins", currentCoins - pendingInvestment); // 투자한 만큼 코인 차감

            coinVestUI.SetActive(false);
            nextButton.gameObject.SetActive(true);

            Debug.Log($"Pending investment: {pendingInvestment} coins in {locationName}");
        });

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            // 'Turns' 값 증가
            int currentTurns = gameManager.GetTurns();
            gameManager.AddTurns(1); // Turn 수 1 증가
            Debug.Log($"Turn increased to: {currentTurns + 1}");

            // 씬 이동
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
        // 외곽선 표시 구현
    }

    private bool IsMouseOverUI()
    {
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
