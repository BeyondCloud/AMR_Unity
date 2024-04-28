using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropDown : MonoBehaviour
{
    public PlayerFunctionCortroller playerController;
    public void HandleInputData(int val)
    {
        switch (val)
        {
            case 0:
                playerController.Stop();
                break;
            case 1:
                playerController.GoForward();
                break;
            case 2:
                playerController.GoBack();
                break;
            case 3:
                playerController.SpinRight();
                break;
            case 4:
                playerController.SpinLeft();
                break;
            case 5:
                playerController.gotoKitchen();
                break;
            case 6:
                playerController.gotoCharge();
                break;
        }
    }
}
