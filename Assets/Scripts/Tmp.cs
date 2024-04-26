// Rotate an object around its Y (upward) axis in response to
// left/right controls.
using UnityEngine;
using System.Collections;

public class Tmp : MonoBehaviour
{
    void FixedUpdate()
    {
        transform.Rotate(0, 100*Time.deltaTime, 0,Space.Self);
    }
}