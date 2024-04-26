using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController: MonoBehaviour
{
    public enum Cmd
    {
        idle = 0,
        go_forward =1,
        go_back =2,
        go_right =3,
        go_left =4,
        go_crowded =5,
        go_charge =6,
        spin_right =7,
        spin_left =8,
        follow =9,
        echo_seen_object =10,
        find =11,
        get_battery_percentage =12,
        dance =13,
        get_speed =14,
        set_speed =15,
        stop =16,
        print=17,
        error=18
    }
    private Cmd _cmd; // Private backing field

    private void HandleControlChange(Cmd newControl)
    {
        switch (newControl)
        {
            case Cmd.idle:
                break;
            case Cmd.go_forward:
                GoForward();
                break;

        }
    }

    public Cmd control
    {
        get { return _cmd; }
        set
        {
            if (_cmd != value) // Only if the value has changed
            {
                _cmd = value;
                HandleControlChange(_cmd);
                Debug.Log(_cmd);
            }
        }
    }

    public Cmd cmd;
    [Header("Movement")]
    
    public float moveSpeed;

    public float groundDrag;

    public float airMultiplier;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    // bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public bool grounded = false;
    private void OnCollisionStay(Collision collision)
    {
        if (grounded == false)
        {
            grounded = true;
        }
    }
 
    private void OnCollisionExit(Collision collision)
    {
        if(grounded == true) // this triggers only if the ball is not touching the ground anymore
        {
            grounded = false;
        }
    }
    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    public void GoForward()
    {
        ResetForce();
        verticalInput = 1;
    }
    private void ResetForce()
    {
        verticalInput = 0;
        horizontalInput = 0;
    }
}