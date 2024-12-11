using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // 이동 속도
    public float interactionRange = 2f; // 상호작용 가능 거리
    public GameObject interactionIndicator; // 상호작용 아이콘
    public GameObject exitUI; // 나가기 UI
    public Button yesButton; // 예 버튼
    public Button noButton; // 아니요 버튼
    public string worldMapSceneName = "WorldMap"; // 이동할 씬 이름

    private Vector3 moveDirection; // 이동 방향
    private GameObject currentInteractable; // 상호작용 가능한 오브젝트
    private bool isExitUIActive = false; // 나가기 UI 활성화 여부
    private bool isInteracting = false; // 상호작용 중 여부

    private Animator animator; // 애니메이터 컴포넌트

    void Start()
    {
        exitUI.SetActive(false); // 시작 시 나가기 UI 비활성화

        // 버튼 이벤트 연결
        yesButton.onClick.AddListener(LoadWorldMapScene);
        noButton.onClick.AddListener(DeactivateExitUI);

        animator = GetComponent<Animator>(); // Animator 컴포넌트 가져오기
    }

    void Update()
    {
        if (!isInteracting) // 상호작용 중이 아닐 때만 이동 처리
        {
            HandleMovement(); // 이동 처리
        }

        CheckForInteractable(); // 상호작용 가능 오브젝트 확인
        HandleInteraction(); // 상호작용 처리
    }

    void HandleMovement()
    {
        if (isExitUIActive) return; // UI 활성화 중일 때 이동 차단

        moveDirection = Vector3.zero;

        // 방향키 입력
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection += Vector3.forward;
            animator.Play("Walk_Backward"); // 뒤로 이동 애니메이션
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection += Vector3.back;
            animator.Play("Walk_Forward"); // 앞으로 이동 애니메이션
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection += Vector3.left;
            animator.Play("Walk_Left"); // 왼쪽 이동 애니메이션
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection += Vector3.right;
            animator.Play("Walk_Right"); // 오른쪽 이동 애니메이션
        }
        else
        {
            animator.Play("Idle"); // 정지 상태 애니메이션
        }

        moveDirection = moveDirection.normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    void CheckForInteractable()
    {
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

        interactionIndicator.SetActive(currentInteractable != null);

        if (currentInteractable != null)
        {
            interactionIndicator.transform.position = transform.position + Vector3.up * 2f; // 머리 위에 표시
        }
    }

    void HandleInteraction()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            // 상호작용 시 애니메이션을 Idle로 설정
            animator.Play("Idle");

            ObjectInteraction interaction = currentInteractable.GetComponent<ObjectInteraction>();

            if (interaction != null)
            {
                isInteracting = true; // 상호작용 시작
                interaction.Interact();
                interaction.OnInteractionEnd += EndInteraction; // 상호작용 종료 이벤트 등록
            }
            else if (currentInteractable.CompareTag("Door"))
            {
                ActivateExitUI();
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
        exitUI.SetActive(true);
    }

    public void DeactivateExitUI()
    {
        isExitUIActive = false;
        exitUI.SetActive(false);
    }

    public void LoadWorldMapScene()
    {
        SceneManager.LoadScene(worldMapSceneName);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    private void EndInteraction()
    {
        isInteracting = false; // 상호작용 종료
    }
}
