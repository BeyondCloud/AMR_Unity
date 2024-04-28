using Unity.VisualScripting;
using UnityEngine;

/*
- Main Object (attach script to this object)
    - Main Camera
    - target (FollowTargetCam)
*/
public class FollowTargetCam : MonoBehaviour
{
    /*
      To initialize the target,
      create an empty GameObject and attach the target
      Do not attach to main object directly
      otherwise, the camera will not spin correctly
    */
    public Transform target;
    public Transform cameraTransform;

    public Vector3 offset; // Offset of the camera from the target
    private float currentX = 0.0f;
    private float currentY = 30.0f;
    public float sensitivityX = 4.0f;
    public float sensitivityY = 1.0f;
    public float scrollSensitivity = 2.0f;
    private const float MIN_ANGLE = -50.0f;
    private const float MAX_ANGLE = 50.0f;
    
    private float distance;

    void Start()
    {
        distance = Vector3.Distance(cameraTransform.position, target.position);
        // currentY = cameraTransform.position.y - target.position.y;
    }
    private void Update()
    {
        transform.rotation = target.rotation;

        if (Input.GetMouseButton(1)) // Right mouse button
        {
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY -= Input.GetAxis("Mouse Y") * sensitivityY;
            currentY = Mathf.Clamp(currentY, MIN_ANGLE, MAX_ANGLE);
            // Debug.Log("currentX: " + currentX + " currentY: " + currentY);
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity, 1f, 5f);
        }
    }
    void LateUpdate()
    {  
        /*
         LateUpdate runs after all 'Update' functions.
         Lastly, the LateUpdate function is commonly used to modify animated model bones
        (ex. making the player model look up and down) or to implement a smooth camera follow.
        */
        if (Input.GetMouseButton(1))
        {
            Vector3 dir = new Vector3(0, 0, -distance);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            cameraTransform.position = target.position + rotation * dir;
        }
        if (Input.GetMouseButtonUp(1)) // Right mouse up
        {
            distance = Vector3.Distance(cameraTransform.position, target.position);
            Vector3 dir = - target.forward * distance;
            Quaternion rotation = Quaternion.Euler(transform.right * 30.0f);
            cameraTransform.position = target.position + rotation * dir;
        }

        cameraTransform.LookAt(target.position);
    }
}