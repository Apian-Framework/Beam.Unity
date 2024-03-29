﻿using System.Collections;
using System.Collections.Generic;
using BeamGameCode;
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
		_main.beamApp.OnSwitchModeReq(BeamModeFactory.kPractice, null);
	}

	public void OnConnectBtn()
	{
		_main.beamApp.OnSwitchModeReq(BeamModeFactory.kNetwork, null);
	}

}
