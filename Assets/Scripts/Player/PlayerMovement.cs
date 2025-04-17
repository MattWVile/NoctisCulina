using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float rotationSpeed = 5.0f; // Speed of the rotation smoothing
    public float moveSpeed = 5.0f; // Speed of the player movement

    public Quaternion currentRotation;

    void Update()
    {
        RotateLightTowardsMouse();
        //MovePlayer();
    }

    void RotateLightTowardsMouse()
    {
        // Get the mouse position in world coordinates
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Set z to 0 since we're working in 2D

        // Calculate the direction from the player to the mouse position
        Vector3 direction = mousePosition - transform.position;

        // Calculate the target angle in degrees
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90;

        // Calculate the current angle
        float currentAngle = transform.eulerAngles.z;

        // Smoothly interpolate the angle
        float angle = Mathf.LerpAngle(currentAngle, targetAngle, rotationSpeed * Time.deltaTime);

        // Apply the rotation to the game object
        transform.rotation = Quaternion.Euler(0, 0, angle);

        currentRotation = Quaternion.Euler(0, 0, angle);
    }

    void MovePlayer()
    {
        // Get input from WASD keys
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calculate movement vector
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0);

        // Apply movement to the player's position
        transform.position += movement * moveSpeed * Time.deltaTime;
    }
}
