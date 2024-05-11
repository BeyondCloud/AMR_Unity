using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 public class RemoveObstructer : MonoBehaviour
    {
        [SerializeField] public Transform target;
        [SerializeField] private float yOffset = 1f;
        [Space]
        [SerializeField] bool debugRay;

        [SerializeField] private List<GameObject> prevHit = new List<GameObject>();    // List of objects which are hidden (determinare by physics hits)

        Vector3 targetPosition;
        private float distance;                                       // Distance between the camera and target, used to limit the RayCast
        private Camera cam;

        void Start()
        {
            distance = Vector3.Distance(transform.position, target.position);
            cam = GetComponent<Camera>();
        }


        void LateUpdate()
        {
            if (debugRay)
                DrawRay();

            ObsctructorManager();
        }

        private void DrawRay()
        {
            targetPosition = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
            distance = Vector3.Distance(targetPosition, transform.position);
            Vector3 direction = transform.position - targetPosition;

            Debug.DrawRay(targetPosition, direction, Color.red);
        }

        private void ObsctructorManager()
        {
            targetPosition = new Vector3(target.position.x, target.position.y + yOffset, target.position.z);
            distance = Vector3.Distance(targetPosition, transform.position);

            RaycastHit[] hits;

             // Cast ray from target.position to camera.position and check if the specified layers sits in the middle
             // Please note I'm not using the LayerMask 'cause at the moment still fail ^_^"
            Ray ray = new Ray(targetPosition, transform.position - targetPosition);
            hits = Physics.RaycastAll(ray, distance);

            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit;
                    hit = hits[i];

                    if (hit.transform.CompareTag("Player"))
                    {
                        continue; }

                    Transform objectHit = hit.transform;

                    // There should be a meshRendereer component on the object we hit
                    if (objectHit.gameObject.GetComponent<MeshRenderer>() == null) continue;

                    if (prevHit.Contains(objectHit.gameObject))
                    {
                        ToggleVisibility(objectHit.gameObject, false);
                    }
                    else
                    {
                        prevHit.Add(objectHit.gameObject);
                        ToggleVisibility(objectHit.gameObject, false);
                    }
                }
            }
            else
            {
                // No hits on the RayCast? Swap the materials back to there original states
                for (int j = prevHit.Count - 1; j >= 0; j--)
                {
                    ToggleVisibility(prevHit[j]);
                    prevHit.Remove(prevHit[j]);
                }
            }

        }
               
        private void ToggleVisibility(GameObject go, bool isVisible = true)
        {
            if (isVisible)
            {
                go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;

            }
            else
            {
                go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
            }
        }
    }