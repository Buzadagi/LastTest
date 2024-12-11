using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // �̵� �ӵ�
    public float interactionRange = 2f; // ��ȣ�ۿ� ���� �Ÿ�
    public GameObject interactionIndicator; // ��ȣ�ۿ� ������
    public GameObject exitUI; // ������ UI
    public Button yesButton; // �� ��ư
    public Button noButton; // �ƴϿ� ��ư
    public string worldMapSceneName = "WorldMap"; // �̵��� �� �̸�

    private Vector3 moveDirection; // �̵� ����
    private GameObject currentInteractable; // ��ȣ�ۿ� ������ ������Ʈ
    private bool isExitUIActive = false; // ������ UI Ȱ��ȭ ����
    private bool isInteracting = false; // ��ȣ�ۿ� �� ����

    private Animator animator; // �ִϸ����� ������Ʈ

    void Start()
    {
        exitUI.SetActive(false); // ���� �� ������ UI ��Ȱ��ȭ

        // ��ư �̺�Ʈ ����
        yesButton.onClick.AddListener(LoadWorldMapScene);
        noButton.onClick.AddListener(DeactivateExitUI);

        animator = GetComponent<Animator>(); // Animator ������Ʈ ��������
    }

    void Update()
    {
        if (!isInteracting) // ��ȣ�ۿ� ���� �ƴ� ���� �̵� ó��
        {
            HandleMovement(); // �̵� ó��
        }

        CheckForInteractable(); // ��ȣ�ۿ� ���� ������Ʈ Ȯ��
        HandleInteraction(); // ��ȣ�ۿ� ó��
    }

    void HandleMovement()
    {
        if (isExitUIActive) return; // UI Ȱ��ȭ ���� �� �̵� ����

        moveDirection = Vector3.zero;

        // ����Ű �Է�
        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveDirection += Vector3.forward;
            animator.Play("Walk_Backward"); // �ڷ� �̵� �ִϸ��̼�
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveDirection += Vector3.back;
            animator.Play("Walk_Forward"); // ������ �̵� �ִϸ��̼�
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection += Vector3.left;
            animator.Play("Walk_Left"); // ���� �̵� �ִϸ��̼�
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection += Vector3.right;
            animator.Play("Walk_Right"); // ������ �̵� �ִϸ��̼�
        }
        else
        {
            animator.Play("Idle"); // ���� ���� �ִϸ��̼�
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
            interactionIndicator.transform.position = transform.position + Vector3.up * 2f; // �Ӹ� ���� ǥ��
        }
    }

    void HandleInteraction()
    {
        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            // ��ȣ�ۿ� �� �ִϸ��̼��� Idle�� ����
            animator.Play("Idle");

            ObjectInteraction interaction = currentInteractable.GetComponent<ObjectInteraction>();

            if (interaction != null)
            {
                isInteracting = true; // ��ȣ�ۿ� ����
                interaction.Interact();
                interaction.OnInteractionEnd += EndInteraction; // ��ȣ�ۿ� ���� �̺�Ʈ ���
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
        isInteracting = false; // ��ȣ�ۿ� ����
    }
}
