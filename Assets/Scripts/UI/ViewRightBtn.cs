using UnityEngine;
using System.Collections;

public class ViewRightBtn : UIBtn  {

	public float lookRadians = -1.57f;
	public float decayRate = .001f;
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
		if (Input.GetKeyDown(KeyCode.X ))
			_main.inputDispatch.LookAround(lookRadians, decayRate);
	}

	public override void doSelect()
	{
		_main.inputDispatch.LookAround(lookRadians, decayRate);
	}
}


