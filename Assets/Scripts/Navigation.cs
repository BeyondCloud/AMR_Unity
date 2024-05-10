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
        charge = 5
    }
    public PlacesEmum places;
    private PlacesEmum oldPlaces;
    // Start is called before the first frame update
    public Transform kitchen;
    public Transform livingRoom;
    public Transform bathroom;

    public Transform bedroom;
    public Transform charge;
    private NavMeshAgent agent;
    public PlayerFunctionCortroller playerController;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (oldPlaces != places)
        {
            if (!agent.enabled)
                agent.enabled = true;
            switch (places)
            {
                case PlacesEmum.idle:
                    if (agent.enabled)
                        agent.ResetPath();
                    break;
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
            }
            oldPlaces = places;
        }
        if (agent.enabled && !agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    playerController.Reset();
                }
            }
        }
    }
    public void SetIdle()
    {
        places = PlacesEmum.idle;
    }
    public void SetTarget(string place)
    {
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
                places = PlacesEmum.charge;
                break;
            default:
                Debug.Log($"Place {place} is not one of kitchen, living_room, bathroom, bedroom or charge_station");
                break;
        }
    }
}
