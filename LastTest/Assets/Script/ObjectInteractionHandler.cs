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

    [Header("Player Data")]
    public int playerKeys = 0; // 플레이어의 열쇠 개수

    private static GameObject activeDescriptionUI; // 현재 활성화된 설명 UI
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // UI 초기화: 비활성화 상태로 설정
        if (descriptionUI != null) descriptionUI.SetActive(false);
        if (unlockPromptUI != null) unlockPromptUI.SetActive(false);
        if (keyAlertUI != null) keyAlertUI.SetActive(false);

        // keyAlertUI 위치 조정 (화면 상단 중앙)
        RectTransform keyAlertRect = keyAlertUI.GetComponent<RectTransform>();
        keyAlertRect.anchoredPosition = new Vector2(0, Screen.height * 0.4f); // 상단 중앙
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
            activeDescriptionUI.SetActive(false); // 기존 UI 비활성화
        }

        activeDescriptionUI = descriptionUI;

        // 장소 정보 설정
        nameText.text = locationName;
        descriptionText.text = locationDescription;

        // 설명 UI 위치 설정 (오브젝트 기준으로 우측 상단)
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 adjustedPosition = new Vector3(screenPosition.x + 150, screenPosition.y + 100, screenPosition.z);
        descriptionUI.transform.position = adjustedPosition;

        descriptionUI.SetActive(true); // 설명 UI 활성화

        // unlockPromptUI 위치 조정: descriptionUI 바로 위에 출력
        if (unlockPromptUI != null)
        {
            RectTransform descriptionRect = descriptionUI.GetComponent<RectTransform>();
            RectTransform unlockPromptRect = unlockPromptUI.GetComponent<RectTransform>();

            // unlockPromptUI를 descriptionUI 위로 위치 조정
            unlockPromptRect.position = new Vector3(descriptionRect.position.x, descriptionRect.position.y + 100, descriptionRect.position.z);

            // unlockPromptUI의 Canvas Sorting Order를 증가시켜 항상 설명 UI 위에 출력
            Canvas unlockCanvas = unlockPromptUI.GetComponent<Canvas>();
            if (unlockCanvas != null)
            {
                unlockCanvas.overrideSorting = true;
                unlockCanvas.sortingOrder = 10; // 높은 Sorting Order 값 설정
            }
        }

        // 버튼 설정
        if (isLocked)
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Lock State"; // 잠금 상태
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(HandleLockedState);
        }
        else
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enter";  // 입장
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(EnterLocation);
        }
    }


    private void HandleLockedState()
    {
        if (playerKeys > 0)
        {
            unlockPromptUI.SetActive(true); // 잠금 해제 UI 활성화
            unlockPromptText.text = "Do you want to use the key to unlock it?";
            unlockYesButton.onClick.RemoveAllListeners();
            unlockYesButton.onClick.AddListener(UnlockLocation);
            unlockNoButton.onClick.RemoveAllListeners();
            unlockNoButton.onClick.AddListener(CloseUnlockPromptUI);
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            keyAlertUI.SetActive(true); // 열쇠 필요 안내 UI 활성화
            keyAlertText.text = "You need a key";
            fadeCoroutine = StartCoroutine(FadeOutKeyAlert());
        }
    }

    private void UnlockLocation()
    {
        playerKeys--; // 열쇠 사용
        isLocked = false; // 잠금 해제
        unlockPromptUI.SetActive(false); // 잠금 해제 UI 비활성화
        ShowDescriptionUI(); // UI 갱신
    }

    private void CloseUnlockPromptUI()
    {
        unlockPromptUI.SetActive(false); // 잠금 해제 UI 비활성화
        CloseDescriptionUI();
    }

    private IEnumerator FadeOutKeyAlert()
    {
        CanvasGroup canvasGroup = keyAlertUI.GetComponent<CanvasGroup>();

        // Alpha 초기값 설정
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        // 서서히 Alpha 값을 줄임
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }

        keyAlertUI.SetActive(false); // UI 비활성화
        canvasGroup.alpha = 1f; // Alpha 초기화
    }

    private void EnterLocation()
    {
        Debug.Log($"{locationName}에 입장합니다!");
        CloseDescriptionUI();
    }

    private void CloseDescriptionUI()
    {
        if (descriptionUI != null)
        {
            descriptionUI.SetActive(false);
        }

        HighlightObject(false); // 외곽선 제거
    }

    private void HighlightObject(bool highlight)
    {
        // 외곽선 표시 구현
    }

    private bool IsMouseOverUI()
    {
        // 마우스가 UI 위에 있는지 확인 (UnityEngine.EventSystems 사용 필요)
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
