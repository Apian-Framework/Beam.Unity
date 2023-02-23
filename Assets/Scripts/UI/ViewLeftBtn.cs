using UnityEngine;
using System.Collections;

public class ViewLeftBtn : UIBtn  {


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
		if (Input.GetKeyDown(KeyCode.Z ))
			_main.inputDispatch.LookAround(PlayStage.lookRadians, PlayStage.lookDecayRate);
	}

	public override void doSelect()
	{
		_main.inputDispatch.LookAround(PlayStage.lookRadians, PlayStage.lookDecayRate);
	}
}

