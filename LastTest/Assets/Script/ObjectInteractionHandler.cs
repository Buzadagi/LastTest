using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ObjectInteractionHandler : MonoBehaviour
{
    [Header("Object Information")]
    public string locationName; // 장소 이름 (Inspector에서 설정)
    public string locationDescription; // 장소 설명 (Inspector에서 설정)
    public bool isLocked = true; // 초기 잠금 상태

    [Header("Global UI References")]
    public GameObject descriptionUI; // 설명 이미지 UI (전역적으로 관리)
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

    private void OnMouseEnter()
    {
        if (CompareTag("Struct"))
        {
            HighlightObject(true); // 외곽선 활성화
        }
    }

    private void OnMouseExit()
    {
        if (CompareTag("Struct"))
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

        nameText.text = locationName; // 장소 이름 설정
        descriptionText.text = locationDescription; // 장소 설명 설정
        descriptionUI.SetActive(true); // 설명 UI 활성화

        // 버튼 설정
        if (isLocked)
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "잠금 상태";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(HandleLockedState);
        }
        else
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "입장";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(EnterLocation);
        }
    }

    private void HandleLockedState()
    {
        if (playerKeys > 0)
        {
            unlockPromptUI.SetActive(true); // 잠금 해제 UI 활성화
            unlockPromptText.text = "열쇠를 사용해 잠금을 해제하시겠습니까?";
            unlockYesButton.onClick.RemoveAllListeners();
            unlockYesButton.onClick.AddListener(UnlockLocation);
            unlockNoButton.onClick.RemoveAllListeners();
            unlockNoButton.onClick.AddListener(CloseUnlockPromptUI);
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            keyAlertUI.SetActive(true); // 열쇠 필요 안내 UI 활성화
            keyAlertText.text = "열쇠가 필요합니다";
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
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1f);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime; // 서서히 투명화
            yield return null;
        }

        keyAlertUI.SetActive(false); // 안내 UI 비활성화
        canvasGroup.alpha = 1f; // 알파값 복원
    }

    private void EnterLocation()
    {
        Debug.Log($"{locationName}에 입장합니다!"); // 입장 처리 (추가 동작 가능)
        CloseDescriptionUI();
    }

    private void CloseDescriptionUI()
    {
        if (descriptionUI != null)
        {
            descriptionUI.SetActive(false); // 설명 UI 비활성화
        }

        HighlightObject(false); // 외곽선 제거
    }

    private void HighlightObject(bool highlight)
    {
        // 외곽선 표시 구현 (기존 Shader나 Material 변경 로직 활용)
    }
}
