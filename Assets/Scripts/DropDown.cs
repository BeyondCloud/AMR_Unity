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
                playerController.GotoKitchen();
                break;
            case 6:
                playerController.GotoCharge();
                break;
            case 7:
                playerController.EchoSeenObjects();
                break;
            case 8:
                playerController.Follow();
                break;
            case 9:
                playerController.GoCrowded();
                break;
            case 10:
                playerController.GoRight();
                break;
            case 11:
                playerController.GoLeft();
                break;
            case 12:
                playerController.GetBatteryPercentage();
                break;
            case 13:
                playerController.GetSpeedLevel();
                break;
            case 14:
                playerController.Dance();
                break;
            case 15:
                playerController.Find("book");
                break;
        }
    }
}
