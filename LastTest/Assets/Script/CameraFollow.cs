using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // �÷��̾� Transform
    public Vector3 offset = new Vector3(0f, 5f, -10f); // ī�޶�� �÷��̾� ���� ��ġ ������
    public Vector3 rotation = new Vector3(30f, 0f, 0f); // ī�޶��� �⺻ ȸ�� ����

    void LateUpdate()
    {
        if (player != null)
        {
            // ī�޶� ��ġ ������Ʈ
            transform.position = player.position + offset;

            // ī�޶� ȸ�� ������Ʈ
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
