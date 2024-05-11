
using UnityEngine;
using TMPro;
using System.Collections;

public class SpeedLevelToText : MonoBehaviour
{
    private TextMeshProUGUI mText; 
    public PlayerKeyboardController playerKeyboardController;
    void Awake()
    {
        mText = gameObject.GetComponent<TextMeshProUGUI>();
        InvokeRepeating("UpdateText", 0, 0.2f);
    }
    void UpdateText()
    {
        mText.text = "Speed Level: " + playerKeyboardController.speedLevel;
    }
}
