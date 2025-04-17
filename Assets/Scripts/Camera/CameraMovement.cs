using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // The player game object to follow
    public Vector3 offset; // Offset from the player's position

    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        // Update the camera's position to follow the player with the specified offset
        transform.position = player.position + offset;
    }
}
