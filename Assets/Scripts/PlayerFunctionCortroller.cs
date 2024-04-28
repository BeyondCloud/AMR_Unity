using UnityEngine;
using UnityEngine.AI;

public class PlayerFunctionCortroller: MonoBehaviour
{
    // public enum Cmd
    // {
    //     idle = 0,
    //     go_forward =1,
    //     go_back =2,
    //     go_right =3,
    //     go_left =4,
    //     go_crowded =5,   <<<<<<<<<<<<<<<
    //     go_charge =6,
    //     spin_right =7,
    //     spin_left =8,
    //     follow =9,    <<<<<<<<<<<<<<
    //     echo_seen_object =10,   <<<<<<<<<<<<<<<<
    //     find =11,
    //     get_battery_percentage =12,
    //     dance =13,
    //     get_speed =14,
    //     set_speed =15,
    //     stop =16,
    //     print=17,
    //     error=18
    // }

    [Header("Movement")]
    public float spin_direction=0;
    private float spin_speed=100;
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
    private NavMeshAgent agent;
    private Navigation navigation;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        navigation = GetComponent<Navigation>();
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

    void FixedUpdate()
    {
        MovePlayer();
        SpinPlayer();

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
    private void SpinPlayer()
    {
        // calculate movement direction
        if(spin_direction != 0)
        {
            transform.Rotate(0, spin_direction * spin_speed*Time.deltaTime, 0,Space.Self);
        }
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
        Stop();
        verticalInput = 1;
    }
    public void GoBack()
    {
        Stop();
        verticalInput = -1;
    }
    public void Stop()
    {
        navigation.SetIdle();
        verticalInput = 0;
        horizontalInput = 0;
        spin_direction = 0;
    }
    public void SpinRight()
    {
        Stop();
        spin_direction = 1;
    }
    public void SpinLeft()
    {
        Stop();
        spin_direction = -1;
    }

    public void gotoKitchen()
    {
        Stop();
        navigation.GoToKitchen();
    }

    public void gotoCharge()
    {
        Stop();
        navigation.GoToCharge();
    }


}