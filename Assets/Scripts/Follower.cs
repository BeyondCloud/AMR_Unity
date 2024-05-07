using UnityEngine;

public class Follower : MonoBehaviour
{
    private Transform target;
    public bool isFollowing = false;
    public float nearDistance = 1.5f;
    public float rotate_speed = 1.0f;
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
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotate_speed * Time.deltaTime, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            newDirection.y = 0.0f;
            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.position = Vector3.MoveTowards(transform.position, modified_position, playerController.speedLevel * Time.deltaTime);
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
