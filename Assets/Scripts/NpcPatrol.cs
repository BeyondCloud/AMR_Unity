using UnityEngine;
using UnityEngine.AI;

public class NpcPatrol:MonoBehaviour
{
    NavMeshAgent agent;
    bool walkPointSet=false;
    public Transform destPoint;

    [SerializeField] LayerMask groudLayer;
    [SerializeField] float range;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    void Update()
    {
        Patrol();
    }
    private void OnCollisionEnter(Collision collision)
    {
        walkPointSet = false;
    }
    void Patrol()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        else
        {
            agent.SetDestination(destPoint.position);
        }
        if (Vector3.Distance(transform.position, destPoint.position) <= 1f)
        {
            walkPointSet = false;
        }
    }
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(-range, range);
        float randomX = Random.Range(-range, range);
        destPoint.position = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        Vector3 direction = (destPoint.position - transform.position).normalized;
        if (Physics.Raycast(destPoint.position, direction, groudLayer))
        {
            walkPointSet = true;
        }
    }
}
