using UnityEngine;
using System.Collections;

public class ViewRightBtn : UIBtn  {
	
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
		if (Input.GetKeyDown(KeyCode.X ))
		{
			// _main.gameCamera.SendCmd((int)GameCamera.ModeBikeView.Commands.kLookAround, 
			// 	(object)new GameCamera.ModeBikeView.LookParams(lookRadians, decayRate) );
		}
	}

	public override void doSelect()
	{
		// _main.gameCamera.SendCmd((int)GameCamera.ModeBikeView.Commands.kLookAround, 
		// 	(object)new GameCamera.ModeBikeView.LookParams(lookRadians, decayRate) );
	}
}


