using UnityEngine;

public class LocationSceneManager : MonoBehaviour
{
    public static Vector3 destinationPosition; // ObjectInteractionHandler에서 전달받을 destinationPosition

    private GameObject player; // Location Scene의 Player 오브젝트

    void Start()
    {
        // Player 오브젝트 찾기
        player = GameObject.Find("Player");

        if (player == null)
        {
            Debug.LogError("Player 오브젝트를 찾을 수 없습니다.");
            return;
        }

        // Player를 destinationPosition으로 이동
        MovePlayerToDestination();
    }

    /// <summary>
    /// Player를 destinationPosition으로 이동시키는 함수
    /// </summary>
    private void MovePlayerToDestination()
    {
        if (destinationPosition != Vector3.zero)
        {
            player.transform.position = destinationPosition;
            Debug.Log($"Player가 {destinationPosition}으로 이동했습니다.");
        }
        else
        {
            Debug.LogWarning("destinationPosition 값이 초기화되지 않았습니다.");
        }
    }
}
