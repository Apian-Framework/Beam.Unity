using UnityEngine;
using System.Collections;
using BeamGameCode;

public class PracticeBtn : UIBtn  {

	protected BeamMain _main = null;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		_main = BeamMain.GetInstance();

	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Return))
			_main.beamApp.OnSwitchModeReq(BeamModeFactory.kPractice, null);
	}

	public override void doSelect()
	{
		_main.beamApp.OnSwitchModeReq(BeamModeFactory.kPractice, null);
	}
}

