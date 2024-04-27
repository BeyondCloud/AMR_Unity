using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMouseControl : MonoBehaviour
{
    public ThirdPersonCameraController cameraController;
    public Transform cameraTransform;
    
    // Start is called before the first frame update    
    private void LateUpdate()
    {
        if (Input.GetMouseButton(1)){ // Right mouse button
            Vector3 forward = cameraTransform.forward;
            forward.y = 0; // Keep the player vertica
            // Update player's rotation to match the camera's rotation on the Y-axis
            if (forward.magnitude > 0)
            {
                transform.rotation = Quaternion.Euler(0, cameraController.currentX, 0);
            }
        }
    }

}