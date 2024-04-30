using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0,360)]
    public float angle;

    public GameObject[] targetRefs;

    public LayerMask targetMask;
    public LayerMask obstructionMask;

    public bool[] canSeeTargets;

    private void Start()
    {
        targetRefs = GameObject.FindGameObjectsWithTag("Findable");
        StartCoroutine(FOVRoutine());
        canSeeTargets = new bool[targetRefs.Length];
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);

        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        for (int i = 0; i < rangeChecks.Length;i++)
        {
            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[i].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                        canSeeTargets[i] = true;
                    else
                        canSeeTargets[i] = false;
                }
                else
                    canSeeTargets[i] = false;
            }
            else if (canSeeTargets[i])
                canSeeTargets[i] = false;
        }

    }
}