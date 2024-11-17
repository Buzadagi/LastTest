using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 플레이어 이동 속도
    public float interactionRange = 2f; // 상호작용 가능 거리
    public GameObject interactionIndicator; // 상호작용 가능 이미지 오브젝트
    public GameObject exitUI; // 나가기 UI
    public Button yesButton; // 예 버튼
    public Button noButton; // 아니요 버튼
    public string worldMapSceneName = "WorldMap"; // 이동할 Scene 이름

    private Vector3 moveDirection; // 이동 방향
    private GameObject currentInteractable; // 현재 상호작용 가능한 오브젝트
    private bool isExitUIActive = false; // 나가기 UI 활성화 여부

    void Start()
    {
        exitUI.SetActive(false); // 시작 시 나가기 UI 비활성화

        // 버튼 이벤트 연결
        yesButton.onClick.AddListener(LoadWorldMapScene);
        noButton.onClick.AddListener(DeactivateExitUI);
    }

    void Update()
    {
        if (isExitUIActive)
        {
            return; // UI가 활성화된 동안 플레이어 입력 차단
        }

        HandleMovement(); // 이동 처리
        CheckForInteractable(); // 상호작용 가능 오브젝트 확인
        HandleInteraction(); // 상호작용 처리
    }

    void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        moveDirection = new Vector3(moveX, 0f, moveZ).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void CheckForInteractable()
    {
        // 상호작용 가능한 오브젝트를 찾음
        currentInteractable = null;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, interactionRange);

        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Interaction") || collider.CompareTag("Door"))
            {
                currentInteractable = collider.gameObject;
                break;
            }
        }

        // 상호작용 가능 여부에 따라 표시 조정
        if (currentInteractable != null)
        {
            interactionIndicator.SetActive(true);
            interactionIndicator.transform.position = transform.position + Vector3.up * 2f; // 플레이어 머리 위에 표시
        }
        else
        {
            interactionIndicator.SetActive(false);
        }
    }

    void HandleInteraction()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            if (currentInteractable.CompareTag("Door"))
            {
                ActivateExitUI(); // 나가기 UI 활성화
            }
            else
            {
                Debug.Log($"Interacted with {currentInteractable.name}");
            }
        }
    }

    void ActivateExitUI()
    {
        isExitUIActive = true;
        exitUI.SetActive(true); // 나가기 UI 활성화
    }

    public void DeactivateExitUI()
    {
        isExitUIActive = false;
        exitUI.SetActive(false); // 나가기 UI 비활성화
    }

    public void LoadWorldMapScene()
    {
        SceneManager.LoadScene(worldMapSceneName); // 설정된 Scene으로 이동
    }

    private void OnDrawGizmosSelected()
    {
        // 상호작용 범위를 시각적으로 확인 (에디터에서만 보임)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
