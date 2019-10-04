using UnityEngine;
using System.Collections;

public class StartBtn : UIBtn  {
	
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
			_main.setGameMode(OldGameMain.ModeID.kPlay);	
	}

	public override void doSelect()
	{
		_main.setGameMode(OldGameMain.ModeID.kPlay);
	}
}

