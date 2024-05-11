
using UnityEngine;
using TMPro;
using System.Collections;

public class SpeedLevelToText : MonoBehaviour
{
    private TextMeshProUGUI mText; 
    public PlayerFunctionCortroller playerFunctionCortroller;
    void Awake()
    {
        mText = gameObject.GetComponent<TextMeshProUGUI>();
        InvokeRepeating("UpdateText", 0, 0.2f);
    }
    void UpdateText()
    {
        int timeout = playerFunctionCortroller.GetTimeOut();
        string timeoutText;
        if (timeout <= 0)
            timeoutText = "no";
        else
            timeoutText = $"{timeout}s";
        mText.text = (
             $"Speed Level: {playerFunctionCortroller.GetSpeedLevel()}\n"
           + $"Timeout: {timeoutText}"
        );
    }
}
