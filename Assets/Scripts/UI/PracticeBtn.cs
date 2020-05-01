using UnityEngine;
using System.Collections;
using BeamBackend;

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
			_main.core.OnSwitchModeReq(BeamModeFactory.kPractice, null);
	}

	public override void doSelect()
	{
		_main.core.OnSwitchModeReq(BeamModeFactory.kPractice, null);
	}
}

