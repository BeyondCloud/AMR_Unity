using UnityEngine;

public class Follower : MonoBehaviour
{
    private Vector3 target;
    public bool isFollowing = false;
    public float nearDistance = 1.5f;
    public float rotate_speed = 1.0f;
    private float distance;
    private PlayerKeyboardController playerController;
    void Start()
    {
        playerController = GetComponent<PlayerKeyboardController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!isFollowing)
        {
            return;
        }
        distance = Vector3.Distance(target, transform.position);
        if (distance > nearDistance)
        {
            Vector3 targetDirection = target - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, rotate_speed * Time.deltaTime, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(transform.position, newDirection, Color.red);

            newDirection.y = 0.0f;
            transform.rotation = Quaternion.LookRotation(newDirection);
            transform.position = Vector3.MoveTowards(transform.position, target, playerController.speedLevel * Time.deltaTime);
        }
    }
    public void SetTarget(Vector3 newTarget)
    {
        newTarget.y = transform.position.y;
        target = newTarget;
        isFollowing = true;
    }
    public void Reset()
    {
        isFollowing = false;
    }
}
