using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class NpcPatrol:MonoBehaviour
{
    NavMeshAgent agent;
    bool walkPointSet=false;
    public Transform destPoint;
    private DateTime lastCollisionEnter = DateTime.Now; 
    private float maxCollisionStayTime = 4.0f;
    [SerializeField] LayerMask groudLayer;
    private float range= 3.0f;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        InvokeRepeating("Patrol", 0, 0.5f);
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        lastCollisionEnter = DateTime.Now;
        walkPointSet = false;
    }
    private void OnCollisionStay(Collision collision)
    {
        if ((DateTime.Now - lastCollisionEnter).TotalSeconds > maxCollisionStayTime)
        {
            walkPointSet = false;
            lastCollisionEnter = DateTime.Now;
        }
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
        float randomZ = UnityEngine.Random.Range(0, range) + 2.0f;
        float randomX = UnityEngine.Random.Range(0, range) + 2.0f;
        //random set positive or negative
        randomZ *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
        randomX *= UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;
        destPoint.position = new Vector3(destPoint.position.x + randomX, 0, destPoint.position.z + randomZ);
        Vector3 direction = (destPoint.position - transform.position).normalized;
        RaycastHit hit;
        //get raycast hit point
        if (Physics.Raycast(transform.position, direction, out hit, groudLayer))
        {
            var hitPoint = hit.point;
            hitPoint.y = transform.position.y;
            destPoint.position = hitPoint;
            walkPointSet = true;
        }

    }
}
