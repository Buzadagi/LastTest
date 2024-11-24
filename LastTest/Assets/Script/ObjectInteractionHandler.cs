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

    [Header("Player Data")]
    public int playerKeys = 0; // �÷��̾��� ���� ����

    private static GameObject activeDescriptionUI; // ���� Ȱ��ȭ�� ���� UI
    private Coroutine fadeCoroutine;

    private void Start()
    {
        // UI �ʱ�ȭ: ��Ȱ��ȭ ���·� ����
        if (descriptionUI != null) descriptionUI.SetActive(false);
        if (unlockPromptUI != null) unlockPromptUI.SetActive(false);
        if (keyAlertUI != null) keyAlertUI.SetActive(false);

        // keyAlertUI ��ġ ���� (ȭ�� ��� �߾�)
        RectTransform keyAlertRect = keyAlertUI.GetComponent<RectTransform>();
        keyAlertRect.anchoredPosition = new Vector2(0, Screen.height * 0.4f); // ��� �߾�
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
            activeDescriptionUI.SetActive(false); // ���� UI ��Ȱ��ȭ
        }

        activeDescriptionUI = descriptionUI;

        // ��� ���� ����
        nameText.text = locationName;
        descriptionText.text = locationDescription;

        // ���� UI ��ġ ���� (������Ʈ �������� ���� ���)
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 adjustedPosition = new Vector3(screenPosition.x + 150, screenPosition.y + 100, screenPosition.z);
        descriptionUI.transform.position = adjustedPosition;

        descriptionUI.SetActive(true); // ���� UI Ȱ��ȭ

        // unlockPromptUI ��ġ ����: descriptionUI �ٷ� ���� ���
        if (unlockPromptUI != null)
        {
            RectTransform descriptionRect = descriptionUI.GetComponent<RectTransform>();
            RectTransform unlockPromptRect = unlockPromptUI.GetComponent<RectTransform>();

            // unlockPromptUI�� descriptionUI ���� ��ġ ����
            unlockPromptRect.position = new Vector3(descriptionRect.position.x, descriptionRect.position.y + 100, descriptionRect.position.z);

            // unlockPromptUI�� Canvas Sorting Order�� �������� �׻� ���� UI ���� ���
            Canvas unlockCanvas = unlockPromptUI.GetComponent<Canvas>();
            if (unlockCanvas != null)
            {
                unlockCanvas.overrideSorting = true;
                unlockCanvas.sortingOrder = 10; // ���� Sorting Order �� ����
            }
        }

        // ��ư ����
        if (isLocked)
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Lock State"; // ��� ����
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(HandleLockedState);
        }
        else
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enter";  // ����
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(EnterLocation);
        }
    }


    private void HandleLockedState()
    {
        if (playerKeys > 0)
        {
            unlockPromptUI.SetActive(true); // ��� ���� UI Ȱ��ȭ
            unlockPromptText.text = "Do you want to use the key to unlock it?";
            unlockYesButton.onClick.RemoveAllListeners();
            unlockYesButton.onClick.AddListener(UnlockLocation);
            unlockNoButton.onClick.RemoveAllListeners();
            unlockNoButton.onClick.AddListener(CloseUnlockPromptUI);
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            keyAlertUI.SetActive(true); // ���� �ʿ� �ȳ� UI Ȱ��ȭ
            keyAlertText.text = "You need a key";
            fadeCoroutine = StartCoroutine(FadeOutKeyAlert());
        }
    }

    private void UnlockLocation()
    {
        playerKeys--; // ���� ���
        isLocked = false; // ��� ����
        unlockPromptUI.SetActive(false); // ��� ���� UI ��Ȱ��ȭ
        ShowDescriptionUI(); // UI ����
    }

    private void CloseUnlockPromptUI()
    {
        unlockPromptUI.SetActive(false); // ��� ���� UI ��Ȱ��ȭ
        CloseDescriptionUI();
    }

    private IEnumerator FadeOutKeyAlert()
    {
        CanvasGroup canvasGroup = keyAlertUI.GetComponent<CanvasGroup>();

        // Alpha �ʱⰪ ����
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(1f);

        // ������ Alpha ���� ����
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime;
            yield return null;
        }

        keyAlertUI.SetActive(false); // UI ��Ȱ��ȭ
        canvasGroup.alpha = 1f; // Alpha �ʱ�ȭ
    }

    private void EnterLocation()
    {
        Debug.Log($"{locationName}�� �����մϴ�!");
        CloseDescriptionUI();
    }

    private void CloseDescriptionUI()
    {
        if (descriptionUI != null)
        {
            descriptionUI.SetActive(false);
        }

        HighlightObject(false); // �ܰ��� ����
    }

    private void HighlightObject(bool highlight)
    {
        // �ܰ��� ǥ�� ����
    }

    private bool IsMouseOverUI()
    {
        // ���콺�� UI ���� �ִ��� Ȯ�� (UnityEngine.EventSystems ��� �ʿ�)
        return UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
    }
}
