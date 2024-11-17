using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ObjectInteractionHandler : MonoBehaviour
{
    [Header("Object Information")]
    public string locationName; // ��� �̸� (Inspector���� ����)
    public string locationDescription; // ��� ���� (Inspector���� ����)
    public bool isLocked = true; // �ʱ� ��� ����

    [Header("Global UI References")]
    public GameObject descriptionUI; // ���� �̹��� UI (���������� ����)
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

    private void OnMouseEnter()
    {
        if (CompareTag("Struct"))
        {
            HighlightObject(true); // �ܰ��� Ȱ��ȭ
        }
    }

    private void OnMouseExit()
    {
        if (CompareTag("Struct"))
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

        nameText.text = locationName; // ��� �̸� ����
        descriptionText.text = locationDescription; // ��� ���� ����
        descriptionUI.SetActive(true); // ���� UI Ȱ��ȭ

        // ��ư ����
        if (isLocked)
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "��� ����";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(HandleLockedState);
        }
        else
        {
            actionButton.GetComponentInChildren<TextMeshProUGUI>().text = "����";
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(EnterLocation);
        }
    }

    private void HandleLockedState()
    {
        if (playerKeys > 0)
        {
            unlockPromptUI.SetActive(true); // ��� ���� UI Ȱ��ȭ
            unlockPromptText.text = "���踦 ����� ����� �����Ͻðڽ��ϱ�?";
            unlockYesButton.onClick.RemoveAllListeners();
            unlockYesButton.onClick.AddListener(UnlockLocation);
            unlockNoButton.onClick.RemoveAllListeners();
            unlockNoButton.onClick.AddListener(CloseUnlockPromptUI);
        }
        else
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            keyAlertUI.SetActive(true); // ���� �ʿ� �ȳ� UI Ȱ��ȭ
            keyAlertText.text = "���谡 �ʿ��մϴ�";
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
        canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1f);

        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.deltaTime; // ������ ����ȭ
            yield return null;
        }

        keyAlertUI.SetActive(false); // �ȳ� UI ��Ȱ��ȭ
        canvasGroup.alpha = 1f; // ���İ� ����
    }

    private void EnterLocation()
    {
        Debug.Log($"{locationName}�� �����մϴ�!"); // ���� ó�� (�߰� ���� ����)
        CloseDescriptionUI();
    }

    private void CloseDescriptionUI()
    {
        if (descriptionUI != null)
        {
            descriptionUI.SetActive(false); // ���� UI ��Ȱ��ȭ
        }

        HighlightObject(false); // �ܰ��� ����
    }

    private void HighlightObject(bool highlight)
    {
        // �ܰ��� ǥ�� ���� (���� Shader�� Material ���� ���� Ȱ��)
    }
}
