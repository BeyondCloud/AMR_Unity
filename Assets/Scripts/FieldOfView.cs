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
    public class Target
    {
        public string name;
        public float distance=0.0f;
        public Vector3 direction=Vector3.zero;
        public bool canSee=false;
    }
    public Target[] targets;

    private void Start()
    {
        targetRefs = GameObject.FindGameObjectsWithTag("Findable");
        StartCoroutine(FOVRoutine());
        targets = new Target[targetRefs.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = new Target();
            targets[i].name = targetRefs[i].name;
        }
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
                targets[i].direction = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, targets[i].direction) < angle / 2)
                {
                    targets[i].distance = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, targets[i].direction, targets[i].distance, obstructionMask))
                        targets[i].canSee = true;
                    else
                        targets[i].canSee = false;
                }
                else
                    targets[i].canSee = false;
            }
            else if (targets[i].canSee)
                targets[i].canSee = false;
        }

    }
}