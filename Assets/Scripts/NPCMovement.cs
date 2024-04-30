using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public float moveSpeed = 0.0f;
    private Rigidbody rb;
    private Animator anim;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }   
    // Update is called once per frame
    void Update()
    {
        moveSpeed = rb.velocity.magnitude;
        anim.SetFloat("speed", moveSpeed);
    }
}
