using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum PlacesEmum
{
    kitchen=0,
    livingRoom=1,
    bathroom=2
}
public class NavigationScript : MonoBehaviour
{
    public PlacesEmum places;
    // Start is called before the first frame update
    public Transform kitchen;
    public Transform livingRoom;
    public Transform bathroom;
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
    }
}
