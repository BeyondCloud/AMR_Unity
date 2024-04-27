using Unity.VisualScripting;
using UnityEngine;

public class FollowTargetCam : MonoBehaviour
{
    private float distance;
    //　To initialize the target,
    // create an empty GameObject and attach the target
    //[;]
    public Transform target;
    public Transform cameraTransform;
    public bool allow_mouse_view_spin = false;

    public Vector3 offset; // Offset of the camera from the target
    private float currentX = 0.0f;
    private float currentY = 30.0f;
    public float sensitivityX = 4.0f;
    public float sensitivityY = 1.0f;
    public float scrollSensitivity = 2.0f;
    private const float MIN_ANGLE = -50.0f;
    private const float MAX_ANGLE = 50.0f;
    void Start()
    {
        distance = Vector3.Distance(cameraTransform.position, target.position);
        // currentY = cameraTransform.position.y - target.position.y;
    }
    void Update()
    {
        transform.rotation = target.rotation;
        if (allow_mouse_view_spin  && Input.GetMouseButton(1)) // Right mouse button
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY -= Input.GetAxis("Mouse Y") * sensitivityY;
            currentY = Mathf.Clamp(currentY, MIN_ANGLE, MAX_ANGLE);
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity, 1f, 5f);
        }
    }
    void LateUpdate()
    {  
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        cameraTransform.position = target.position + rotation * dir;
        cameraTransform.LookAt(target.position);
    }
}