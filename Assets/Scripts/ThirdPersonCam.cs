using UnityEngine;
public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform player; // Player's Transform
    public float distance = 5.0f; // Distance from the player
    public float currentX = 0.0f;
    public float currentY = 20.0f;
    public float sensitivityX = 4.0f;
    public float sensitivityY = 1.0f;
    public float scrollSensitivity = 2.0f;
    private const float MIN_ANGLE = -50.0f;
    private const float MAX_ANGLE = 50.0f;
    private void Update()
    {
        if (Input.GetMouseButton(1)) // Right mouse button
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY -= Input.GetAxis("Mouse Y") * sensitivityY;
            currentY = Mathf.Clamp(currentY, MIN_ANGLE, MAX_ANGLE);
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity, 1f, 10f);
        }
    }
    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = player.position + rotation * dir;
        transform.LookAt(player.position);

        // Update player's rotation to match the camera's rotation on the Y-axis
        Vector3 forward = transform.forward;
        forward.y = 0; // Keep the player vertical
        if (forward.magnitude > 0)
        {
            player.rotation = Quaternion.LookRotation(forward);
        }
    }
}