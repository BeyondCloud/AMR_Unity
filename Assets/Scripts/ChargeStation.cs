using UnityEngine;

public class ChargeStation : MonoBehaviour
{
    public float chargeRate = 10f; // Amount of power charged per second

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure the collider is tagged as "Player"
        {
            // Start charging power (you could also start a coroutine here)
            other.GetComponent<Cleaner>().isCharging = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop charging power
            other.GetComponent<Cleaner>().isCharging = false;
        }
    }
}