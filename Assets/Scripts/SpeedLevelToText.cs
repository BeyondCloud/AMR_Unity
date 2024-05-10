
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
    }
    
    void Start()
    {
    }

    void FixedUpdate()
    {
        mText.text = "Speed Level: " + playerKeyboardController.speedLevel;
    }
}
