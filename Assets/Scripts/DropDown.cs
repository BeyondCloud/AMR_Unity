using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropDown : MonoBehaviour
{
    public PlayerController playerController;
    public void HandleInputData(int val)
    {
        if (val == 1)
        {
            playerController.GoForward();
        }
    }
}
