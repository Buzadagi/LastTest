using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestionButtonHandler : MonoBehaviour
{
    public Button questionButton; // "���� Ȯ��" ��ư
    public RectTransform questionListImage; // ���� ��� �̹��� (UI Panel)
    public float animationDuration = 0.3f; // �ִϸ��̼� �ð�
    public float maxHeight = 130f; // ���� ����� �ִ� ����

    private bool isExpanded = false; // ���� ���� (���������� ����)
    private Coroutine currentCoroutine; // ���� ���� ���� Coroutine

    private void Start()
    {
        // Pivot�� �������� ���� (Inspector���� ���� ����)
        questionListImage.pivot = new Vector2(0.5f, 1f);

        // �ʱ� ���� ����
        questionListImage.sizeDelta = new Vector2(questionListImage.sizeDelta.x, 0); // ���̸� 0���� ����
        questionListImage.gameObject.SetActive(false); // ��Ȱ��ȭ

        // ��ư Ŭ�� �̺�Ʈ ���
        questionButton.onClick.AddListener(ToggleQuestionList);
    }

    private void ToggleQuestionList()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine); // ���� �ִϸ��̼� ����
        }

        if (isExpanded)
        {
            // ����
            currentCoroutine = StartCoroutine(AnimateHeight(0, false));
        }
        else
        {
            // ��ġ��
            questionListImage.gameObject.SetActive(true); // ��Ȱ��ȭ ������ ��� ���� Ȱ��ȭ
            currentCoroutine = StartCoroutine(AnimateHeight(maxHeight, true));
        }

        isExpanded = !isExpanded; // ���� ���
    }

    private IEnumerator AnimateHeight(float targetHeight, bool activateAfter)
    {
        float startHeight = questionListImage.sizeDelta.y;
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float newHeight = Mathf.Lerp(startHeight, targetHeight, elapsedTime / animationDuration);
            questionListImage.sizeDelta = new Vector2(questionListImage.sizeDelta.x, newHeight);
            yield return null;
        }

        questionListImage.sizeDelta = new Vector2(questionListImage.sizeDelta.x, targetHeight); // ���� ���� ����

        if (!activateAfter)
        {
            questionListImage.gameObject.SetActive(false); // ���� �Ϸ� �� ��Ȱ��ȭ
        }
    }
}
