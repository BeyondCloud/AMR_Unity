
using UnityEngine;
using UnityEngine.UI;

public class BatteryUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Cleaner cleaner;
    public Image PowerBar;


    void Update()
    {
        PowerBar.fillAmount = cleaner.power / 100;
    }
}
