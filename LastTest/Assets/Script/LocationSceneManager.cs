using UnityEngine;

public class LocationSceneManager : MonoBehaviour
{
    public static Vector3 destinationPosition; // ObjectInteractionHandler���� ���޹��� destinationPosition

    private GameObject player; // Location Scene�� Player ������Ʈ

    void Start()
    {
        // Player ������Ʈ ã��
        player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogError("Player ������Ʈ�� ã�� �� �����ϴ�.");
            return;
        }

        // Player�� destinationPosition���� �̵�
        MovePlayerToDestination();
    }

    /// <summary>
    /// Player�� destinationPosition���� �̵���Ű�� �Լ�
    /// </summary>
    private void MovePlayerToDestination()
    {
        if (destinationPosition != Vector3.zero)
        {
            player.transform.position = destinationPosition;
            Debug.Log($"Player�� {destinationPosition}���� �̵��߽��ϴ�.");
        }
        else
        {
            Debug.LogWarning("destinationPosition ���� �ʱ�ȭ���� �ʾҽ��ϴ�.");
        }
    }
}
