using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

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

    public void LocalPlayerBikeLeft() => feMain.backend.OnTurnReq(localPlayerBike.bb.bikeId, TurnDir.kLeft);
    public void LocalPlayerBikeRight() => feMain.backend.OnTurnReq(localPlayerBike.bb.bikeId, TurnDir.kRight);    
    public void ToggleCamHeight() => feMain.gameCamera.SendCmd((int)GameCamera.ModeBikeView.Commands.kToggleHighLow, null );
    public void LookAround(float angleRad, float decayRate)
    {
	    feMain.gameCamera.SendCmd((int)GameCamera.ModeBikeView.Commands.kLookAround, 
		 	(object)new GameCamera.ModeBikeView.LookParams(angleRad, decayRate) );    
    }
    
}
