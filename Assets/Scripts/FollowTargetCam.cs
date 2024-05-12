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
    public Transform cameraOrigin;

    public Vector3 offset; // Offset of the camera from the target
    private float currentX;
    private float currentY;
    
    public float sensitivityX = 4.0f;
    public float sensitivityY = 1.0f;
    public float scrollSensitivity = 2.0f;
    private const float MIN_ANGLE = -50.0f;
    private const float MAX_ANGLE = 50.0f;
    
    private float distance;
    void Start()
    {
        distance = Vector3.Distance(cameraTransform.position, target.position);
        currentY = Vector3.Angle(cameraTransform.position - target.position, -target.forward);
        currentX = Vector3.SignedAngle(Vector3.forward, target.forward, Vector3.up);
    }
    private void Update()
    {
        transform.rotation = target.rotation;
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity, 1f, 5f);
        }
        Debug.DrawLine(target.position,target.position + target.forward, Color.red);
    }
    void LateUpdate()
    {  
        /*
         LateUpdate runs after all 'Update' functions.
         Lastly, the LateUpdate function is commonly used to modify animated model bones
        (ex. making the player model look up and down) or to implement a smooth camera follow.
        */
        Quaternion rotation;
        Vector3 dir = new Vector3(0, 0, -distance);
        if (Input.GetMouseButtonDown(1))
        {   
            currentY = Vector3.Angle(cameraTransform.position - target.position, -target.forward);
            currentX = Vector3.SignedAngle(Vector3.forward, target.forward, Vector3.up); //(0, 0, 1)
            // Debug.Log("currentX: " + currentX);
        }
        else if (Input.GetMouseButton(1))
        {
            // Input.GetAxis use click point as the origin (0,0)
            // Input.GetAxis range between -1...1 
            currentX += Input.GetAxis("Mouse X") * sensitivityX;
            currentY -= Input.GetAxis("Mouse Y") * sensitivityY;
            currentY = Mathf.Clamp(currentY, MIN_ANGLE, MAX_ANGLE);
            rotation = Quaternion.Euler(currentY, currentX, 0);
            cameraTransform.position = target.position + rotation * dir;
        }
        else
        {
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, cameraOrigin.position, Time.deltaTime * 3.0f);
        }
        cameraTransform.LookAt(target.position);
    }
}