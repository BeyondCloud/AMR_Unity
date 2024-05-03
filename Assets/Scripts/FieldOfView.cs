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
        public Transform transform;
        public bool canSee=false;
        public Target(string name, Transform transform)
        {
            this.name = name;
            this.transform = transform;
        }
    }
    public Target[] targets;

    private void Start()
    {
        targetRefs = GameObject.FindGameObjectsWithTag("Findable");
        StartCoroutine(FOVRoutine());
        targets = new Target[targetRefs.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = new Target(targetRefs[i].name, targetRefs[i].transform);
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
                targets[i].transform = rangeChecks[i].transform;
                Transform target = rangeChecks[i].transform;
                Vector3 direction = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, direction) < angle / 2)
                {
                    float distance = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, direction, distance, obstructionMask))
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