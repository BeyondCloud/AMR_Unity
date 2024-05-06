using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class WaitForCompletion : CustomYieldInstruction
{
    IEnumerator coroutine;

    public WaitForCompletion(IEnumerator coroutine)
    {
        this.coroutine = coroutine;
    }

    public override bool keepWaiting
    {
        get { return coroutine.MoveNext(); }
    }
}
public class ScanMeta
{
    public float angle;
    public Dictionary<string, List<Vector3>> objects;
    public ScanMeta(float angle, Dictionary<string, List<Vector3>> objects)
    {
        this.angle = angle;
        this.objects = objects;
    }
}
public class PlayerFunctionCortroller : MonoBehaviour
{
    // public enum Cmd
    // {
    //     idle = 0,     //stop == idle
    //     go_forward =1,
    //     go_back =2,
    //     go_right =3,
    //     go_left =4,
    //     go_crowded =5,   
    //     go_charge =6,
    //     spin_right =7,
    //     spin_left =8,
    //     follow =9,    
    //     echo_seen_object =10,   
    //     get_battery_percentage =12,
    //     get_speed =14,

    //     find =11, <<<<<<
    //     dance =13,



    // OTHERS
    //     set_speed =15,
    //     error=18
    //     print=17,
    // }

    [Header("Movement")]
    public float spin_direction = 0;
    private float spin_speed = 100;


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
    private int moveSpeed = 400;
    private PlayerKeyboardController playerController;
    private Cleaner cleaner;
    private bool isDancing = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        navigation = GetComponent<Navigation>();
        fov = GetComponent<FieldOfView>();
        follower = GetComponent<Follower>();
        playerController = GetComponent<PlayerKeyboardController>();
        cleaner = GetComponent<Cleaner>();
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
        if (grounded == true) // this triggers only if the ball is not touching the ground anymore
        {
            grounded = false;
        }
    }
    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }
    private void SpinPlayer()
    {
        // calculate movement direction
        if (spin_direction != 0)
        {
            transform.Rotate(0, spin_direction * spin_speed * Time.deltaTime, 0, Space.Self);
        }
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
    public void Stop()
    {
        StopAllCoroutines();
        agent.enabled = true;
        navigation.SetIdle();
        agent.enabled = false;
        verticalInput = 0;
        horizontalInput = 0;
        spin_direction = 0;
        follower.Reset();
        isDancing = false;
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
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
    IEnumerator Rotate(float degree)
    {
        yield return new WaitForCompletion(_Rotate(degree));
    }
    public void GoRight()
    {
        Stop();
        StartCoroutine(Rotate(90));
        verticalInput = 1;
    }
    public void GoLeft()
    {
        Stop();
        StartCoroutine(Rotate(-90));
        verticalInput = 1;
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

    public void GotoKitchen()
    {
        Stop();
        agent.enabled = true;
        navigation.GoToKitchen();
    }

    public void GotoCharge()
    {
        Stop();
        agent.enabled = true;
        navigation.GoToCharge();
    }
    private Dictionary<string, List<Vector3>> scanObjects()
    {
        var dict = new Dictionary<string, List<Vector3>>();
        for (int i = 0; i < fov.targetsInView.Count; i++)
        {

            if (!dict.ContainsKey(fov.targetsInView[i].name))
            {
                dict[fov.targetsInView[i].name] = new List<Vector3>();
            }
            dict[fov.targetsInView[i].name].Add(fov.targetsInView[i].position);

        }
        return dict;
    }
    public void EchoSeenObjects()
    {
        Dictionary<string, List<Vector3>> dict = scanObjects();
        string log = "";
        foreach (var item in dict)
        {
            log += item.Value.Count + " " + item.Key + "\n";
        }
        if (log == "")
            Debug.Log("No objects in sight");
        else
            Debug.Log("I see a" + log);
    }
    public void Follow()
    {
        Stop();
        float min_distance = 1000;
        int min_idx = -1;
        for (int i = 0; i < fov.targetsInView.Count; i++)
        {
            if (fov.targetsInView[i].name == "person")
            {
                float distance = Vector3.Distance(
                            transform.position,
                            fov.targetsInView[i].position
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
            Debug.Log("Following " + fov.targetsInView[min_idx].name);
            follower.SetTarget(fov.targetsInView[min_idx].position);
        }
    }
    IEnumerator _Rotate(float angle)
    {
        Vector3 rotate_to = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
        while (Vector3.Angle(transform.forward, rotate_to) > 1)
        {
            var rotation = Vector3.RotateTowards(transform.forward, rotate_to, Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(rotation);
            yield return null;
        } 
    }

    IEnumerator go_crowded_routine()
    {
        List<ScanMeta> scanMetas = new List<ScanMeta>();
        int split = 4;
        // get surrounding objects
        if (split < 1)
        {
            throw new System.ArgumentException("split should be at least one");
        }
        else if (split > 8)
        {
            throw new System.ArgumentException("split should be at most 8");
        }
        float angle = 360 / split;
        float current_angle = 0;
        for (int i = 0; i < split; i++)
        {
            Dictionary<string, List<Vector3>> cv_objects = scanObjects();
            Debug.Log("Found " + cv_objects.Count + " objects");
            scanMetas.Add(new ScanMeta(angle: current_angle, objects: cv_objects));
            current_angle += angle;
            yield return new WaitForCompletion(_Rotate(angle));
        }

        // find most crowded area
        int max_person = -1;
        int index_max_person = -1;
        for (int i = 0; i < scanMetas.Count; i++)
        {
            ScanMeta scan_res = scanMetas[i];
            if (scan_res.objects.ContainsKey("person") && scan_res.objects["person"].Count > max_person)
            {
                max_person = scan_res.objects["person"].Count;
                index_max_person = i;
            }
        }
        if (max_person == -1)
        {
            Debug.Log("No person in sight");
            yield break;
        }
        Vector3 centroid = new Vector3(0, 0, 0);
        foreach (Vector3 obj in scanMetas[index_max_person].objects["person"])
        {
            centroid += obj;
        }
        centroid /= max_person;
        //OnGoto
        follower.SetTarget(centroid);
        while (follower.isFollowing)
        {
            yield return null;
        }
        follower.Reset();
    }
    public void GoCrowded()
    {
        Stop();
        StartCoroutine(go_crowded_routine());
    }
    public int GetBatteryPercentage()
    {
        int res = cleaner.GetBatteryPercentage();
        Debug.Log("Battery percentage: " + res + "%");
        return res;
    }
    public void SetSpeedLevel(int speedLevel)
    {
        playerController.speedLevel = speedLevel;
    }
    public int GetSpeedLevel()
    {
        return playerController.speedLevel;
    }
    IEnumerator dance_routine()
    {
        while (isDancing)
        {
            transform.Rotate(0, 1, 0, Space.Self);
            yield return null;
        }
    }
    public void Dance()
    {
        Stop();
        isDancing = true;
        Debug.Log("Dancing");
    }

}