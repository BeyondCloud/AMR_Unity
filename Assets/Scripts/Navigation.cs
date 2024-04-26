using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Navigation : MonoBehaviour
{
    public enum PlacesEmum
    {
        stay =0,
        kitchen=1,
        livingRoom=2,
        bathroom=3,
        bedroom=4
    }
    public PlacesEmum places;
    // Start is called before the first frame update
    public Transform kitchen;
    public Transform livingRoom;
    public Transform bathroom;

    public Transform bedroom;
    private NavMeshAgent agent;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(places == PlacesEmum.kitchen)
            agent.destination = kitchen.position;
        else if(places == PlacesEmum.livingRoom)
            agent.destination = livingRoom.position;
        else if(places == PlacesEmum.bathroom)
            agent.destination = bathroom.position;
        else if(places == PlacesEmum.bedroom)
            agent.destination = bedroom.position;
    }
    public void GoToKitchen()
    {
        places = PlacesEmum.kitchen;
    }
    public void GoToLivingRoom()
    {
        places = PlacesEmum.livingRoom;
    }
    public void GoToBathroom()
    {
        places = PlacesEmum.bathroom;
    }
    public void GoToBedroom()
    {
        places = PlacesEmum.bedroom;
    }
}
