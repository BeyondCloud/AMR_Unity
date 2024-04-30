using UnityEngine;

public class Follower : MonoBehaviour
{
    private Transform target=null;
    public float nearDistance = 1.5f;
    public float move_speed = 2.0f;
    public float rotate_speed = 1.0f;
    private float distance;
    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }
        distance = Vector3.Distance(target.position, transform.position);
        if (distance > nearDistance)
        {
            Vector3 targetDirection = target.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotate_speed  * Time.deltaTime, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            newDirection.y = 0.0f;
            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.position = Vector3.MoveTowards(transform.position, target.position, move_speed * Time.deltaTime); 
        }
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
    public void Reset()
    {
        target = null;
    }
}
