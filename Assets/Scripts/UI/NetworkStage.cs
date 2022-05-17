using System.Collections;
using System.Collections.Generic;
using BeamGameCode;
using UnityEngine;

public class NetworkStage : MonoBehaviour
{
	protected BeamMain _main = null;

	// Use this for initialization
	protected void Start ()
	{
		_main = BeamMain.GetInstance();
	}

	public void OnProceedBtn()
	{
		_main.beamApp.OnPushModeReq(BeamModeFactory.kNetPlay, null);
	}

	public void OnCancelBtn()
	{
		_main.beamApp.OnSwitchModeReq(BeamModeFactory.kSplash, null);
	}

}
