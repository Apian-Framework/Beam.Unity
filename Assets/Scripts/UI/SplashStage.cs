using System.Collections;
using System.Collections.Generic;
using BeamBackend;
using UnityEngine;

public class SplashStage : MonoBehaviour
{
	protected BeamMain _main = null;

	// Use this for initialization
	protected void Start ()
	{
		_main = BeamMain.GetInstance();
	}

	public void OnPracticeBtn()
	{
		Debug.Log("OnPracticeButton()");
		_main.core.OnSwitchModeReq(BeamModeFactory.kPractice, null);
	}

	public void OnConnectBtn()
	{
		_main.core.OnSwitchModeReq(BeamModeFactory.kConnect, null);
	}

	public void OnStartBtn()
	{
		_main.core.OnSwitchModeReq(BeamModeFactory.kPlay, null);
	}

}
