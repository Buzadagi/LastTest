using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // 플레이어 Transform
    public Vector3 offset = new Vector3(0f, 5f, -10f); // 카메라와 플레이어 간의 위치 오프셋
    public Vector3 rotation = new Vector3(30f, 0f, 0f); // 카메라의 기본 회전 각도

    void LateUpdate()
    {
        if (player != null)
        {
            // 카메라 위치 업데이트
            transform.position = player.position + offset;

            // 카메라 회전 업데이트
            transform.rotation = Quaternion.Euler(rotation);
        }
    }
}
