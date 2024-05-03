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
        public Vector3 position;
        public Target(string name, Vector3 position)
        {
            this.name = name;
            this.position = position;
        }
    }
    // public Target[] targets;
    public List<Target> targetsInView=new List<Target>();

    private void Start()
    {
        targetRefs = GameObject.FindGameObjectsWithTag("Findable");
        StartCoroutine(FOVRoutine());
        // targets = new Target[targetRefs.Length];
        // for (int i = 0; i < targets.Length; i++)
        // {
        //     targets[i] = new Target(targetRefs[i].name, targetRefs[i].transform.Find("metarig"));
        //     Debug.Log(targetRefs[i].transform.Find("metarig").position);
        // }
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
        targetsInView = new List<Target>();
        for (int i = 0; i < rangeChecks.Length;i++)
        {
            if (rangeChecks.Length != 0)
            {
                Transform target = rangeChecks[i].transform;
                Vector3 direction = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, direction) < angle / 2)
                {
                    float distance = Vector3.Distance(transform.position, target.position);
                    if (!Physics.Raycast(transform.position, direction, distance, obstructionMask))
                    {
                        Target t = new Target(rangeChecks[i].gameObject.name, target.position);
                        targetsInView.Add(t);
                    }
                }
            }
        }

    }
}