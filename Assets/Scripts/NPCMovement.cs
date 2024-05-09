using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed = 0.0f;
    private Animator anim;
    Vector3 prevPos;
    void Awake()
    {
        anim = GetComponent<Animator>();
        prevPos = transform.position;
    }   
    // Update is called once per frame
    void Update()
    {
        float speed = Vector3.Distance(transform.position, prevPos)/Time.deltaTime;
        anim.SetFloat("speed", speed);
        prevPos = transform.position;
    }
}
