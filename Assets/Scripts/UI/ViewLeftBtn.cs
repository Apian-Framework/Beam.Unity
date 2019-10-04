using UnityEngine;
using System.Collections;

public class ViewLeftBtn : UIBtn  {
	

	public float lookRadians = 1f;
	public float decayRate = 1f;

	protected OldGameMain _main = null;	

	// Use this for initialization
	protected override void Start () 
	{
		base.Start();		
		_main = (OldGameMain)utils.findObjectComponent("GameMain", "GameMain");	
	}
		
	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Z ))
		{
			_main.gameCamera.SendCmd((int)GameCamera.ModeBikeView.Commands.kLookAround, 
				(object)new GameCamera.ModeBikeView.LookParams(lookRadians, decayRate) );
		}
	}

	public override void doSelect()
	{
		_main.gameCamera.SendCmd((int)GameCamera.ModeBikeView.Commands.kLookAround, 
			(object)new GameCamera.ModeBikeView.LookParams(lookRadians, decayRate) );
	}
}

