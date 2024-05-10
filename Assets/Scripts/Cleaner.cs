using UnityEngine;

public class Cleaner : MonoBehaviour
{
    public float power = 100f;
    private float drainRate = 0.3f;
    public bool isCharging = false;
    private float charge_rate = 5f;
    private float max_power = 100f;

    private void Update()
    {
        if (!isCharging && power > 0)
        {
            power -= drainRate * Time.deltaTime; // Continuously drain power
        }else if (isCharging && power < max_power)
        {
            power += charge_rate * Time.deltaTime; 
        }
    }
    public int GetBatteryPercentage()
    {
        return Mathf.RoundToInt(100*(power / max_power));
    }
}