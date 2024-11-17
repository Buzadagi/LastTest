using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // �÷��̾� �̵� �ӵ�
    public float interactionRange = 2f; // ��ȣ�ۿ� ���� �Ÿ�
    public GameObject interactionIndicator; // ��ȣ�ۿ� ���� �̹��� ������Ʈ
    public GameObject exitUI; // ������ UI
    public Button yesButton; // �� ��ư
    public Button noButton; // �ƴϿ� ��ư
    public string worldMapSceneName = "WorldMap"; // �̵��� Scene �̸�

    private Vector3 moveDirection; // �̵� ����
    private GameObject currentInteractable; // ���� ��ȣ�ۿ� ������ ������Ʈ
    private bool isExitUIActive = false; // ������ UI Ȱ��ȭ ����

    void Start()
    {
        exitUI.SetActive(false); // ���� �� ������ UI ��Ȱ��ȭ

        // ��ư �̺�Ʈ ����
        yesButton.onClick.AddListener(LoadWorldMapScene);
        noButton.onClick.AddListener(DeactivateExitUI);
    }

    void Update()
    {
        if (isExitUIActive)
        {
            return; // UI�� Ȱ��ȭ�� ���� �÷��̾� �Է� ����
        }

        HandleMovement(); // �̵� ó��
        CheckForInteractable(); // ��ȣ�ۿ� ���� ������Ʈ Ȯ��
        HandleInteraction(); // ��ȣ�ۿ� ó��
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
        // ��ȣ�ۿ� ������ ������Ʈ�� ã��
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

        // ��ȣ�ۿ� ���� ���ο� ���� ǥ�� ����
        if (currentInteractable != null)
        {
            interactionIndicator.SetActive(true);
            interactionIndicator.transform.position = transform.position + Vector3.up * 2f; // �÷��̾� �Ӹ� ���� ǥ��
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
                ActivateExitUI(); // ������ UI Ȱ��ȭ
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
        exitUI.SetActive(true); // ������ UI Ȱ��ȭ
    }

    public void DeactivateExitUI()
    {
        isExitUIActive = false;
        exitUI.SetActive(false); // ������ UI ��Ȱ��ȭ
    }

    public void LoadWorldMapScene()
    {
        SceneManager.LoadScene(worldMapSceneName); // ������ Scene���� �̵�
    }

    private void OnDrawGizmosSelected()
    {
        // ��ȣ�ۿ� ������ �ð������� Ȯ�� (�����Ϳ����� ����)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
