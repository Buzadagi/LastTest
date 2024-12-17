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
    public int buildingNumber; // 건물 번호

    [Header("Global UI References")]
    public GameObject descriptionUI; // 설명 이미지 UI
    public GameObject popUpImage; // 팝업 이미지 UI
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
    public GameObject coinImg; // CoinImg UI (기존에 있는 이미지)
    public TextMeshProUGUI coinText; // CoinImg 안의 CoinText

    [Header("GameManager Reference")]
    private GameManager gameManager; // GameManager 참조
    private NPCManager npcManager; // NPCManager 참조

    private static GameObject activeDescriptionUI; // 현재 활성화된 설명 UI
    private Coroutine fadeCoroutine;

    private int currentInvestment = 0; // Coin Vest에서 설정한 금액
    private int pendingInvestment = 0; // NextButton 클릭 전까지 관리할 임시 금액

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        npcManager = FindObjectOfType<NPCManager>(); // NPCManager를 찾아서 참조

        if (descriptionUI != null) descriptionUI.SetActive(false);
        if (unlockPromptUI != null) unlockPromptUI.SetActive(false);
        if (keyAlertUI != null) keyAlertUI.SetActive(false);
        if (coinVestUI != null) coinVestUI.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(false); // NextButton 초기화
        if (coinImg != null) coinImg.SetActive(false); // CoinImg는 초기에는 비활성화
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
        // "Struct" 태그가 있는 객체에서만 실행
        if (CompareTag("Struct"))
        {
            // NPCManager를 찾고 chosenLocation이 현재 buildingNumber와 일치하는지 확인
            if (npcManager != null)
            {
                bool npcFound = false; // NPC가 현재 위치에 있는지 확인할 변수

                foreach (var npc in npcManager.npcList)
                {
                    if (npc.chosenLocation == buildingNumber)
                    {
                        // NPC가 해당 위치에 있을 때
                        Debug.Log($"현재 위치에 NPC가 있습니다! NPC 이름: {npc.npcName}");
                        ShowDescriptionUI(); // NPC가 위치한 곳에서 UI를 표시
                        npcFound = true;
                        break; // NPC가 이미 해당 위치에 있을 경우 더 이상 진행하지 않음
                    }
                }

                if (CompareTag("Struct"))
                {
                    ShowDescriptionUI();
                }
            }
        }
    }


    /*
    private void OnMouseDown()
    {
        if (CompareTag("Struct"))
        {
            Debug.Log("현재 위치에 NPC가 있습니다!");
            ShowDescriptionUI();
        }
    }*/

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

            // CoinImg 활성화 및 배팅한 수치 표시
            if (coinImg != null)
            {
                coinImg.SetActive(true);
                if (coinText != null)
                {
                    coinText.text = $"{pendingInvestment}"; // 배팅한 코인 수치 표시
                }
            }

            nextButton.gameObject.SetActive(true);

            // 버튼 위치 초기화 (UI 기준)
            RectTransform nextButtonRect = nextButton.GetComponent<RectTransform>();
            nextButtonRect.anchoredPosition = Vector2.zero; // UI 캔버스 좌표계 기준으로 위치 설정

            // 버튼이 항상 상위 계층에 표시되도록 설정
            nextButton.transform.SetAsLastSibling();

            // 버튼의 CanvasGroup 활성화 (선택 사항)
            CanvasGroup nextButtonCanvasGroup = nextButton.GetComponent<CanvasGroup>();
            if (nextButtonCanvasGroup != null)
            {
                nextButtonCanvasGroup.alpha = 1f;
                nextButtonCanvasGroup.interactable = true;
                nextButtonCanvasGroup.blocksRaycasts = true;
            }

            Debug.Log($"Pending investment: {pendingInvestment} coins in {locationName}");
        });

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(() =>
        {
            // 'Turns' 값 증가
            int currentTurns = gameManager.GetTurns();
            gameManager.AddTurns(1); // Turn 수 1 증가
            Debug.Log($"Turn increased to: {currentTurns + 1}");

            // NPCManager에서 건물 번호에 해당하는 위치를 가져와서 이동
            if (npcManager != null && npcManager.buildingLocations.Count > buildingNumber)
            {
                Transform targetLocation = npcManager.buildingLocations[buildingNumber];

                // 유저의 pendingInvestment와 겹치는 NPC의 betAmount 비교
                NPCManager.NPCInfo npcAtLocation = null;
                foreach (var npc in npcManager.npcList)
                {
                    if (npc.chosenLocation == buildingNumber)
                    {
                        npcAtLocation = npc;
                        break;
                    }
                }

                // npcAtLocation이 null이 아니면 비교 작업 진행
                if (npcAtLocation != null)
                {
                    Debug.Log($"유저의 pendingInvestment: {pendingInvestment}, NPC {npcAtLocation.npcName}의 betAmount: {npcAtLocation.betAmount}");

                    if (pendingInvestment >= npcAtLocation.betAmount)
                    {
                        // pendingInvestment가 더 크거나 같으면 씬 이동
                        SceneMove.LoadSceneWithPosition(sceneName, targetLocation.position);
                    }
                    else
                    {
                        // betAmount가 더 크면 씬 이동하지 않고 버튼 비활성화
                        nextButton.gameObject.SetActive(false); // NextButton 비활성화
                        coinImg.SetActive(false); // Coin Img 비활성화
                        Debug.Log("NPC의 betAmount가 더 큽니다. 유저는 다른 장소를 선택해야 합니다.");

                        // 이미지 팝업 활성화 및 알파값 줄이기
                        StartCoroutine(ShowMessageAndFadeOut()); // 메시지 표시하고 알파값 줄이기
                    }
                }
                else
                {
                    // 해당 위치에 NPC가 없을 때 씬 이동
                    SceneMove.LoadSceneWithPosition(sceneName, targetLocation.position);
                }
            }

            // 씬 이동
            // SceneMove.LoadSceneWithPosition(sceneName, destinationPosition);
        });
    }

    // Coroutine을 이용해 알파값 천천히 줄이기
    private IEnumerator ShowMessageAndFadeOut()
    {
        // 이미지 팝업 활성화
        popUpImage.SetActive(true);  // 알파값을 조절할 이미지 활성화

        // 알파값 초기화
        Image image = popUpImage.GetComponent<Image>();
        Color startColor = image.color;
        startColor.a = 1f;  // 초기 알파값 1로 설정
        image.color = startColor;

        // 알파값을 천천히 줄여 0으로 만듦
        float fadeDuration = 2f; // 페이드 아웃 시간 (2초)
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration); // 1에서 0으로 천천히 변화
            image.color = new Color(startColor.r, startColor.g, startColor.b, alpha); // 알파값 적용
            yield return null;
        }

        // 알파값이 0이 되면 이미지 비활성화
        popUpImage.SetActive(false);
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
