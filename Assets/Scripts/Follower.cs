using UnityEngine;
using UnityEngine.AI;

public class Follower : MonoBehaviour
{
    private Transform target;
    public bool isFollowing = false;
    private const float nearDistance = 1.5f;
    private const float rotate_speed = 1.0f;
    private const float near_rotate_speed = 2.5f;
    
    private float follow_speed = 1.0f;
    private float distance;
    private GameObject dummyTarget;
    private PlayerKeyboardController playerController;
    void Start()
    {
        playerController = GetComponent<PlayerKeyboardController>();
        dummyTarget = new GameObject();
    }
    // Update is called once per frame
    void Update()
    {
        if (!isFollowing)
        {
            return;
        }
        distance = Vector3.Distance(target.position, transform.position);
        if (distance > nearDistance)
        {
            Vector3 modified_position = target.position;
            modified_position.y = transform.position.y;
            Vector3 targetDirection = modified_position - transform.position;
            
            var rot_speed=(distance < 2.0f)? near_rotate_speed: rotate_speed;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rot_speed * Time.deltaTime, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            newDirection.y = 0.0f;
            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.position = Vector3.MoveTowards(transform.position, modified_position, follow_speed*playerController.speedLevel * Time.deltaTime);
        }
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        isFollowing = true;
    }
    
    public void SetTarget(Vector3 newTarget)
    {
        dummyTarget.transform.position = newTarget;
        target = dummyTarget.transform;
        isFollowing = true;
    }
    
    public void Reset()
    {
        isFollowing = false;
    }
}
