using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropDown : MonoBehaviour
{
    public PlayerController playerController;
    public void HandleInputData(int val)
    {
        if (val == 0)
        {
            playerController.Stop();
        }
        else if (val == 1)
        {
            playerController.GoForward();
        }
        else if (val == 2)
        {
            playerController.GoBack();
        }
        else if (val == 3)
        {
            playerController.SpinRight();
        }
        else if (val == 4)
        {
            playerController.gotoKitchen();
        }
    }
}
