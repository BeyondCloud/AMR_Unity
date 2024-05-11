using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerKeyboardController : MonoBehaviour
{
    public PlayerFunctionCortroller playerController;
    [Header("Movement")]
    public int speedLevel=3;
    private int maxSpeedLevel = 5;
    private int speedUnit = 30;
    private int baseSpeed = 300;
    private float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    // bool grounded;

    public Transform thirdPersonCam;


    float horizontalInput;
    float verticalInput;
    Rigidbody rb;
    private NavMeshAgent agent;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        rb.freezeRotation = true;
        readyToJump = true;
        SetSpeed(speedLevel);
    }

    private void Update()
    {
        MyInput();
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

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        // if(Input.GetKey(jumpKey) && readyToJump && grounded)
        if(Input.GetKey(jumpKey) && readyToJump)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
        // when q is pressed
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerController.SpinLeft();
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            playerController.SpinRight();
        }
        // if any key is up
        if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
        {
            playerController.Reset();
        }
        
    }
    public bool grounded = false;
    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (grounded == false)
    //     {
    //         grounded = true;
    //     }
    // }
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
        Vector3 moveDirection = thirdPersonCam.position - transform.position;
        moveDirection.y = 0;
        Quaternion rotation = Quaternion.Euler(0,90,0);
        Vector3 moveDirectionNormal = rotation * moveDirection;
        moveDirection = - moveDirection * verticalInput - moveDirectionNormal * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        
    }
    // private void MovePlayer()
    // {
    //     float rotationSpeed = 3.0f;
    //     float moveSpeed = 4.0f;
    //     Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
    //     movementDirection.Normalize();
    //     transform.Translate(movementDirection * moveSpeed * Time.deltaTime, Space.World);
        
    //     if (movementDirection != Vector3.zero)
    //     {
    //         Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
    //         transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);            
    //     }
    // }

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

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    public void SetSpeed(int speedLevel)
    {
        this.speedLevel = Mathf.Clamp(speedLevel, 1, maxSpeedLevel);
        moveSpeed = baseSpeed + speedUnit*speedLevel;
    }
    
}