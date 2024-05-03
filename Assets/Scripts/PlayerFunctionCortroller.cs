using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using System.Collections;


public class ScanMeta
{
    public float angle;
    public Dictionary<string, List<Transform>> objects;
    public ScanMeta(float angle, Dictionary<string, List<Transform>> objects)
    {
        this.angle = angle;
        this.objects = objects;
    }
}
public class PlayerFunctionCortroller : MonoBehaviour
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
    public float spin_direction = 0;
    private float spin_speed = 100;
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
    private int coroutine_count = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        navigation = GetComponent<Navigation>();
        fov = GetComponent<FieldOfView>();
        follower = GetComponent<Follower>();
        // StartCoroutine(OnRotate(90));
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
        if (coroutine_count > 0)
            return;

        MovePlayer();
        SpinPlayer();
    }

    public bool grounded = false;
    public bool toggle_collide = false; // Use to break coroutine
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        toggle_collide = !toggle_collide;
    }
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
    private Dictionary<string, List<Transform>> scanObjects()
    {
        var dict = new Dictionary<string, List<Transform>>();
        for (int i = 0; i < fov.targets.Length; i++)
        {
            var target = fov.targets[i];
            if (target.canSee)
            {
                if (!dict.ContainsKey(target.name))
                {
                    dict[target.name] = new List<Transform>();
                }
                dict[target.name].Add(target.transform);
            }
        }
        return dict;
    }
    public void echoSeenObjects()
    {
        Dictionary<string, List<Transform>> dict = scanObjects();
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
            follower.SetTarget(fov.targets[min_idx].transform);
        }
    }
    // public void rotate(int angle)
    // {
    //     StartCoroutine(OnRotate(angle));
    // }
    // IEnumerator OnRotate(float angle)
    // {
    //     coroutine_count += 1;
    //     Vector3 rot = new Vector3(0, angle, 0);
    //     Vector3 rotate_to = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
    //     while (Vector3.Angle(transform.forward, rotate_to) > 1)
    //     {
    //         var rotation = Vector3.RotateTowards(transform.forward, rotate_to, Time.deltaTime, 0.0f);
    //         transform.rotation = Quaternion.LookRotation(rotation);
    //         yield return null;
    //     }
    //     coroutine_count -= 1;
    // }
    // IEnumerator OnGoto(Vector3 target)
    // {
    //     coroutine_count += 1;
    //     var step = moveSpeed * Time.deltaTime; // calculate distance to move
    //     bool init_toggle_collide = toggle_collide;
    //     // if collide, break coroutine
    //     while (Vector3.Distance(transform.position, target) > 0.1 || toggle_collide == init_toggle_collide)
    //     {
    //         transform.position = Vector3.MoveTowards(transform.position, target, step);
    //         yield return null;
    //     }
    //     coroutine_count -= 1;
    // }
    // private List<ScanMeta> get_surrounding_objects(int split = 4)
    // {
    //     /*
    //     Spin clockwise
    //     Turn 360 / split times and get list of object list
    //     Return angle and object list
    //     for example:

    //             cup
    //              0

    //     270     BOT       90

    //             180

    //     return [
    //         (0, {"cup":1}),
    //         (90, []),
    //         (180, []),
    //         (270, []),
    //     ]
    //     */
    //     if (split < 1)
    //     {
    //         throw new System.ArgumentException("split should be at least one");
    //     }
    //     else if (split > 8)
    //     {
    //         throw new System.ArgumentException("split should be at most 8");
    //     }
    //     float angle = 360 / split;
    //     float current_angle = 0;
    //     List<ScanMeta> res = new List<ScanMeta>();
    //     for (int i = 0; i < split; i++)
    //     {
    //         Debug.Log("Rotating " + angle);
    //         Dictionary<string, List<Transform>> cv_objects = scanObjects();
    //         res.Add(new ScanMeta(angle: current_angle, objects: cv_objects));
    //         current_angle += angle;

    //         // Rotate
    //         Vector3 rot = new Vector3(0, angle, 0);
    //         Vector3 rotate_to = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
    //         OnRotate(angle);
    //     }
    //     return res;
    // }
    IEnumerator go_crowded_thread()
    {
        coroutine_count += 1;
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
        List<ScanMeta> res = new List<ScanMeta>();
        for (int i = 0; i < split; i++)
        {
            Debug.Log("Rotating " + angle);
            Dictionary<string, List<Transform>> cv_objects = scanObjects();
            res.Add(new ScanMeta(angle: current_angle, objects: cv_objects));
            current_angle += angle;

            // OnRotate
            Vector3 rot = new Vector3(0, angle, 0);
            Vector3 rotate_to = Quaternion.AngleAxis(angle, Vector3.up) * transform.forward;
            while (Vector3.Angle(transform.forward, rotate_to) > 1)
            {
                var rotation = Vector3.RotateTowards(transform.forward, rotate_to, Time.deltaTime, 0.0f);
                transform.rotation = Quaternion.LookRotation(rotation);
                yield return null;
            }
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
        foreach (Transform obj in scanMetas[index_max_person].objects["person"])
        {
            centroid += obj.position;
        }
        centroid /= max_person;
        //OnGoto

        var step = moveSpeed * Time.deltaTime; // calculate distance to move
        bool init_toggle_collide = toggle_collide;
        // if collide, break coroutine
        while (Vector3.Distance(transform.position, centroid) > 0.1 || toggle_collide == init_toggle_collide)
        {
            transform.position = Vector3.MoveTowards(transform.position, centroid, step);
            yield return null;
        }
        coroutine_count -= 1;
    }
    public void go_crowded()
    {
        StartCoroutine(go_crowded_thread());
    }

}