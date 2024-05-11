using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using TMPro;

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
    //     echo_seen_objects =10,
    //     get_battery_percentage =12,
    //     get_speed =14,
    //     dance =13,
    //     find =11,


    // OTHERS
    //     set_speed =15,
    //     error=18
    //     print=17,
    // }
    private int timeOut = 10; // if negative, no timeout
    private DateTime lastCommandTime = DateTime.Now; 
    [Header("Movement")]
    public bool busyFlag = false;
    public float spin_direction = 0;
    private float spin_speed = 100;
    public bool stopOnCollision = false;

    public float groundDrag;

    public float airMultiplier;
    private List<Coroutine> routines = new List<Coroutine>();

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    [Header("Ground Check")]
    public float playerHeight;
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
    public TMPro.TMP_Dropdown dropDown;
    public bool grounded = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        navigation = GetComponent<Navigation>();
        fov = GetComponent<FieldOfView>();
        follower = GetComponent<Follower>();
        playerController = GetComponent<PlayerKeyboardController>();
        cleaner = GetComponent<Cleaner>();
        InvokeRepeating("TimeOutCheck", 0, 1.0f);
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
    public void SetTimeout(int timeout)
    {
        Debug.Log("Set timeout to " + timeout);
        timeOut = timeout;
    }
    void TimeOutCheck()
    {
        if (timeOut < 0)
            return;
        if (IsBusy())
        {
            TimeSpan diff = DateTime.Now - lastCommandTime;
            if (diff.TotalSeconds > timeOut)
            {
                Debug.Log("Timeout");
                Reset();
            }
        }
    }

    public bool IsBusy()
    {
        var res = busyFlag || agent.enabled || routines.Count > 0;
        return res;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
             return;
        if (stopOnCollision)
        {
            Debug.Log("Collision detected, stopping");
            Reset();
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (grounded == false)
        {
            grounded = true;
        }
        if (collision.gameObject.CompareTag("Ground"))
             return;
        if (stopOnCollision)
        {
            Debug.Log("Collision detected, stopping");
            Reset();
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
    public void Reset()
    {
        // Debug.Log("Reset!");
        // dropDown.value = 0; // This will make idle unselectable
        // StopAllCoroutines(); // Don't do this, coroutines need to exit gracefully (ex: text fade out)
        lastCommandTime = DateTime.Now;
        foreach (var routine in routines)
        {
            StopCoroutine(routine);
            // Debug.Log("KILL "+routine.GetHashCode());
        }
        routines.Clear();
        stopOnCollision = false;
        agent.enabled = true;
        navigation.SetIdle();
        agent.enabled = false;
        verticalInput = 0;
        horizontalInput = 0;
        spin_direction = 0;
        follower.Reset();
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        busyFlag = false;
    }
    public void GoForward()
    {
        Reset();
        busyFlag = true;
        stopOnCollision = true;
        verticalInput = 1;
    }
    public void GoBack()
    {
        Reset();
        busyFlag = true;
        stopOnCollision = true;
        verticalInput = -1;
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
    IEnumerator RotateHelper(float degree)
    {
        yield return new WaitForCompletion(_Rotate(degree));
        Reset();
    }
    public void Rotate(float degree)
    {
        Reset();
        routines.Add( StartCoroutine(RotateHelper(degree)));
    }
    IEnumerator RotateAndGoHelper(float degree)
    {
        yield return new WaitForCompletion(_Rotate(degree));
        GoForward();
    }
    private void RotateAndGo(float degree)
    {
        routines.Add(StartCoroutine(RotateAndGoHelper(degree)));
    }
    
    public void GoRight()
    {
        Reset();
        RotateAndGo(90);
    }
    public void GoLeft()
    {
        Reset();
        RotateAndGo(-90);
    }

    public void SpinRight()
    {
        Reset();
        busyFlag = true;
        spin_direction = 1;
    }
    public void SpinLeft()
    {
        Reset();
        busyFlag = true;
        spin_direction = -1;
    }

    public void Goto(string place)
    {
        Reset();
        busyFlag = true;
        agent.enabled = true;
        navigation.SetTarget(place);
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
            dict[fov.targetsInView[i].name].Add(fov.targetsInView[i].transform.position);

        }
        return dict;
    }
    public void EchoSeenObjects()
    {
        Dictionary<string, List<Vector3>> dict = scanObjects();
        string log = "";
        foreach (var item in dict)
        {
            log += item.Value.Count + " " + item.Key + " ";
        }
        if (log == "")
            Debug.Log("No objects in sight");
        else
            Debug.Log("I see " + log);
    }
    public int getFovNearestObjID(string target_name)
    {
        float min_distance = 1000;
        int min_idx = -1;
        for (int i = 0; i < fov.targetsInView.Count; i++)
        {
            if (fov.targetsInView[i].name == target_name)
            {
                float distance = Vector3.Distance(
                            transform.position,
                            fov.targetsInView[i].transform.position
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
            return min_idx;
        }
        return -1;
    }
    public void Find(string target_name, bool stopOnCollision = true)
    {
        Reset();
        busyFlag = true;
        this.stopOnCollision = stopOnCollision;
        int id = getFovNearestObjID(target_name);
        if (id != -1)
        {
            Debug.Log("Found " + target_name + " in sight!");
            navigation.SetTarget(fov.targetsInView[id].transform.position);
        }
        else
        {
            Debug.Log("No " + target_name + " in sight");
            busyFlag = false;
        }
    }
    public void Follow()
    {
        Reset();
        busyFlag = true;
        Find("person",stopOnCollision=false);
    }

    IEnumerator find_surrounding(string target)
    {
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
        List<Vector3> objectsInMaxAngle = new List<Vector3>();
        for (int i = 0; i < split; i++)
        {
            Dictionary<string, List<Vector3>> cv_objects = scanObjects();
            if (cv_objects.ContainsKey(target))
                Debug.Log($"Found {cv_objects[target].Count} {target}(s)");
            else
                Debug.Log($"Found 0 {target}");

            if (cv_objects.ContainsKey(target) && cv_objects[target].Count > objectsInMaxAngle.Count)
            {
                objectsInMaxAngle = cv_objects[target];
            }
            current_angle += angle;
            yield return new WaitForCompletion(_Rotate(angle));
        }

        // find most crowded area
        if (objectsInMaxAngle.Count == 0)
        {
            Debug.Log("No " + target + " in sight");
            yield break;
        }
        Vector3 centroid = new Vector3(0, 0, 0);
        foreach (Vector3 obj in objectsInMaxAngle)
        {
            centroid += obj;
        }
        centroid /= objectsInMaxAngle.Count;
        //OnGoto
        navigation.SetTarget(centroid);
    }
    public void GoCrowded()
    {
        Reset();
        busyFlag = true;
        routines.Add(StartCoroutine(find_surrounding("person")));
    }
    public int GetBatteryPercentage()
    {
        int res = cleaner.GetBatteryPercentage();
        Debug.Log("Battery percentage: " + res + "%");
        return res;
    }
    public void SetSpeedLevel(int speedLevel)
    {
        playerController.SetSpeed(speedLevel);
        Debug.Log("Set Speed Level: " + playerController.speedLevel);
    }
    public int GetSpeedLevel()
    {
        return  playerController.speedLevel;
    }
    public int GetTimeOut()
    {
        return timeOut;
    }
    IEnumerator dance_routine()
    {
        while (true)
        {
            yield return new WaitForCompletion(_Rotate(45));
            yield return new WaitForCompletion(_Rotate(-90));
            yield return new WaitForCompletion(_Rotate(45));
        }
    }
    public void Dance()
    {
        Reset();
        busyFlag = true;
        routines.Add(StartCoroutine(dance_routine()));
    }

}