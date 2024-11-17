using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestionButtonHandler : MonoBehaviour
{
    public Button questionButton; // "질문 확인" 버튼
    public RectTransform questionListImage; // 질문 목록 이미지 (UI Panel)
    public float animationDuration = 0.3f; // 애니메이션 시간
    public float maxHeight = 130f; // 질문 목록의 최대 높이

    private bool isExpanded = false; // 현재 상태 (펼쳐졌는지 여부)
    private Coroutine currentCoroutine; // 현재 실행 중인 Coroutine

    private void Start()
    {
        // Pivot을 위쪽으로 설정 (Inspector에서 설정 가능)
        questionListImage.pivot = new Vector2(0.5f, 1f);

        // 초기 상태 설정
        questionListImage.sizeDelta = new Vector2(questionListImage.sizeDelta.x, 0); // 높이를 0으로 설정
        questionListImage.gameObject.SetActive(false); // 비활성화

        // 버튼 클릭 이벤트 등록
        questionButton.onClick.AddListener(ToggleQuestionList);
    }

    private void ToggleQuestionList()
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine); // 기존 애니메이션 중지
        }

        if (isExpanded)
        {
            // 접기
            currentCoroutine = StartCoroutine(AnimateHeight(0, false));
        }
        else
        {
            // 펼치기
            questionListImage.gameObject.SetActive(true); // 비활성화 상태일 경우 먼저 활성화
            currentCoroutine = StartCoroutine(AnimateHeight(maxHeight, true));
        }

        isExpanded = !isExpanded; // 상태 토글
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

        questionListImage.sizeDelta = new Vector2(questionListImage.sizeDelta.x, targetHeight); // 최종 높이 설정

        if (!activateAfter)
        {
            questionListImage.gameObject.SetActive(false); // 접기 완료 후 비활성화
        }
    }
}
