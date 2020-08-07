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
    public void SwitchCameraView()
    {
        if (localPlayerBike == null)
            return;

        switch (feMain.gameCamera.getMode())
        {
        case GameCamera.CamModeID.kBikeView:
            feMain.gameCamera.StartOverheadMode(localPlayerBike.gameObject);
            break;
        case GameCamera.CamModeID.kOverheadView:
            feMain.gameCamera.StartEnemyView(localPlayerBike.gameObject);
            break;
        default:
        case GameCamera.CamModeID.kEnemyView:
            feMain.gameCamera.StartBikeMode(localPlayerBike.gameObject);
            break;
        }
    }
    public void LookAround(float angleRad, float decayRate)
    {
	    feMain.gameCamera.LookAround(angleRad, decayRate);
    }

}
