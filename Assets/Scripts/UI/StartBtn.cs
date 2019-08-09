using UnityEngine;
using System.Collections;

public class StartBtn : UIBtn  {
	
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
			_main.setGameMode(GameMain.ModeID.kPlay);	
	}

	public override void doSelect()
	{
		_main.setGameMode(GameMain.ModeID.kPlay);
	}
}

