using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


public class Navigation : MonoBehaviour
{
    public enum PlacesEmum
    {
        idle = 0,
        kitchen = 1,
        livingRoom = 2,
        bathroom = 3,
        bedroom = 4,
        charge = 5,
        others = 6
    }
    public PlacesEmum places;
    // Start is called before the first frame update
    public Transform kitchen;
    public Transform livingRoom;
    public Transform bathroom;

    public Transform bedroom;
    public Transform charge;
    private Transform FollowMe;
    private NavMeshAgent agent;
    public PlayerFunctionCortroller playerController;
    bool stopOnReach = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Agent is enabled when the place tag changed to non-idle
        if (places == PlacesEmum.idle)
        {
            if (agent.enabled)
                agent.ResetPath();
            agent.enabled = false;
            return;
        }
        agent.enabled = true;
        switch (places)
        {
            case PlacesEmum.kitchen:
                agent.destination = kitchen.position;
                break;
            case PlacesEmum.livingRoom:
                agent.destination = livingRoom.position;
                break;
            case PlacesEmum.bathroom:
                agent.destination = bathroom.position;
                break;
            case PlacesEmum.bedroom:
                agent.destination = bedroom.position;
                break;
            case PlacesEmum.charge:
                agent.destination = charge.position;
                break;
            case PlacesEmum.others:
                agent.destination = FollowMe.position;
                break;
        }
        if (stopOnReach)
        {
            if (Vector3.Distance(agent.destination, transform.position) <  agent.stoppingDistance+0.1)
            {
                Debug.Log("Reached destination!");
                playerController.Reset();
            }
        }
    }

    public void SetIdle()
    {
        places = PlacesEmum.idle;
    }
    public void SetTarget(string place, bool stopOnReach=false)
    {
        agent.enabled = true;
        this.stopOnReach = stopOnReach;
        Debug.Log("SetTarget: " + place);
        switch (place)
        {
            case "kitchen":
                places = PlacesEmum.kitchen;
                break;
            case "living_room":
                places = PlacesEmum.livingRoom;
                break;
            case "bathroom":
                places = PlacesEmum.bathroom;
                break;
            case "bedroom":
                places = PlacesEmum.bedroom;
                break;
            case "charge_station":
            case "charge":
                places = PlacesEmum.charge;
                break;
            default:
                Debug.Log($"Place {place} is not one of kitchen, living_room, bathroom, bedroom or charge_station");
                playerController.Reset();
                break;
        }
    }
    public void SetTarget(Transform place, bool stopOnReach=false)
    {
        agent.enabled = true;
        this.stopOnReach = stopOnReach;
        places = PlacesEmum.others;
        FollowMe = place;

    }
    public void SetTarget(Vector3 place, bool stopOnReach=false)
    {
        agent.enabled = true;
        this.stopOnReach = stopOnReach;
        places = PlacesEmum.others;
        agent.destination = place;
    }
}
