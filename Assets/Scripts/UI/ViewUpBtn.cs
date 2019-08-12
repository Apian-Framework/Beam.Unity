using UnityEngine;
using System.Collections;

public class ViewUpBtn : UIBtn  {
	
	public float lookRadians = -1f;
	public float decayRate = 1f;
	protected GameMain _main = null;	

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();		
		_main = (GameMain)utils.findObjectComponent("GameMain", "GameMain");	
		
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.S ))
		{
			_main.gameCamera.SendCmd((int)GameCamera.ModeBikeView.Commands.kToggleHighLow, null );
		}
	}

	public override void doSelect()
	{
		_main.gameCamera.SendCmd((int)GameCamera.ModeBikeView.Commands.kToggleHighLow, null);
	}
}


