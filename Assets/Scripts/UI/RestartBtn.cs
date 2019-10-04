using UnityEngine;
using System.Collections;


public class RestartBtn : UIBtn  {
	
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
		if (Input.GetKeyDown(KeyCode.Return))
			_main.SendCmd((int)GameModePlay.Commands.kRespawn, null);	
	}

	public override void doSelect()
	{
		_main.SendCmd((int)GameModePlay.Commands.kRespawn, null);
	}
}

