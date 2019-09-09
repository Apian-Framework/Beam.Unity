using UnityEngine;
using System.Collections;


public class RestartBtn : UIBtn  {
	
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
		if (Input.GetKeyDown(KeyCode.Return))
			_main.SendCmd((int)GameModePlay.Commands.kRespawn, null);	
	}

	public override void doSelect()
	{
		_main.SendCmd((int)GameModePlay.Commands.kRespawn, null);
	}
}

