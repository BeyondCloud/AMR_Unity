using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using System.Collections;

public class DefaultDict<TKey> : Dictionary<TKey, int>
{
    // Constructor
    public DefaultDict() : base() {}

    // Indexer
    public new int this[TKey key]
    {
        get
        {
            if (!this.ContainsKey(key))
            {
                this[key] = 0; // Default value for int
            }
            return base[key];
        }
        set
        {
            base[key] = value;
        }
    }
}
public class PlayerFunctionCortroller: MonoBehaviour
{
    // public enum Cmd
    // {
    //     idle = 0,
    //     go_forward =1,
    //     go_back =2,
    //     go_right =3,
    //     go_left =4,
    //     go_crowded =5,   <<<<<<<<<<<<<<< global crowded is okay, but FOV crowded is better
    //     go_charge =6,
    //     spin_right =7,
    //     spin_left =8,
    //     follow =9,    DONE
    //     echo_seen_object =10,   DONE
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
    private FieldOfView fov;
    private Follower follower;
    private bool is_coroutine_running = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        navigation = GetComponent<Navigation>();
        fov = GetComponent<FieldOfView>();
        follower = GetComponent<Follower>();
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
        if (is_coroutine_running)
            return;
           
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
        agent.enabled = true;
        navigation.SetIdle();
        agent.enabled = false;
        verticalInput = 0;
        horizontalInput = 0;
        spin_direction = 0;
        follower.Reset();
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
        agent.enabled = true;
        navigation.GoToKitchen();
    }

    public void gotoCharge()
    {
        Stop();
        agent.enabled = true;
        navigation.GoToCharge();
    }
    private DefaultDict<string> scanObjects()
    {
        var dict = new DefaultDict<string>();
        for (int i = 0; i < fov.targets.Length; i++)
        {
            if (fov.targets[i].canSee)
            {
                dict[fov.targets[i].name] += 1;
            }
        }
        return dict;
    }
    public void echoSeenObjects()
    {
        DefaultDict<string> dict = scanObjects();
        string log = "";
        foreach (var item in dict)
        {
            log += item.Value + " "  + item.Key + "\n";
        }
        if (log == "")
            Debug.Log("No objects in sight");
        else
            Debug.Log("I see " + log);
    }
    public void follow()
    {
        float min_distance = 1000;
        int min_idx = -1;
        for (int i = 0; i < fov.targets.Length; i++)
        {
            if (fov.targets[i].canSee && fov.targets[i].name == "person")
            {
                float distance = Vector3.Distance(
                            transform.position, 
                            fov.targets[i].transform.position
                        );
                if (distance < min_distance)
                {
                    min_distance = distance;
                    min_idx = i;
                }
            }
        }
        if (min_idx != -1)
        {
            Debug.Log("Following " + fov.targets[min_idx].name);
            follower.SetTarget( fov.targets[min_idx].transform);
        }
    }
    public void rotate(int degree)
    {
        Vector3 rot = new Vector3(0, degree, 0);
        Vector3 rotation = Quaternion.AngleAxis(degree, Vector3.up) * transform.forward;
        StartCoroutine(OnRotate(rotation));
    }
    IEnumerator OnRotate(Vector3 rotate_to)
    {
        is_coroutine_running = true;
        while (Vector3.Angle(transform.forward, rotate_to) > 1)
        {
            var rotation = Vector3.RotateTowards(transform.forward, rotate_to,  Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotation);
            yield return null;
        }
        is_coroutine_running = false;
    }
    public void go_crowded()
    {

    }

}