using UnityEngine;
using System.Collections;

public class ViewUpBtn : UIBtn  {
	
	public float lookRadians = -1f;
	public float decayRate = 1f;
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
		if (Input.GetKeyDown(KeyCode.Space))
			_main.inputDispatch.SwitchCameraView();
	}

	public override void doSelect()
	{
		_main.inputDispatch.SwitchCameraView();
	}
}


