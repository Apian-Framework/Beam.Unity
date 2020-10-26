using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeamGameCode;

public class InputDispatch
{
    protected BeamMain feMain;
    public FePlayerBike localPlayerBike {get; private set; } = null;

    public InputDispatch(BeamMain bm)
    {
        feMain = bm;
    }
    public void SetLocalPlayerBike(GameObject playerBike)
    {
       localPlayerBike = playerBike.transform.GetComponent<FePlayerBike>();
    }

    public void LocalPlayerBikeLeft() => localPlayerBike.RequestTurn(TurnDir.kLeft);
    public void LocalPlayerBikeRight() => localPlayerBike.RequestTurn(TurnDir.kRight);
    public void SwitchCameraView(GameObject focusObj = null)
    {
        focusObj = focusObj ?? localPlayerBike?.gameObject;
        if (focusObj == null)
            return;

        float toastSecs = 3;

        switch (feMain.gameCamera.getMode())
        {
        case GameCamera.CamModeID.kBikeView:
            feMain.gameCamera.StartOverheadMode(focusObj);
            feMain.uiController.ShowToast($"Overhead View", Toast.ToastColor.kGreen, toastSecs, "camtoast");
            break;
        case GameCamera.CamModeID.kOverheadView:
            feMain.gameCamera.StartEnemyView(focusObj);
            feMain.uiController.ShowToast($"Target View", Toast.ToastColor.kGreen, toastSecs, "camtoast");
            break;
        default:
        case GameCamera.CamModeID.kEnemyView:
            feMain.gameCamera.StartBikeMode(focusObj);
            feMain.uiController.ShowToast($"Follow View", Toast.ToastColor.kGreen, toastSecs, "camtoast");
            break;
        }
    }
    public void LookAround(float angleRad, float decayRate)
    {
	    feMain.gameCamera.LookAround(angleRad, decayRate);
    }

}
