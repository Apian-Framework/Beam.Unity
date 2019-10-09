using UnityEngine;
using System.Collections;

public class StartBtn : UIBtn  {
	
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
// Hmm - not sure what to do here		
//		if (Input.GetKeyDown(KeyCode.Return))
//			_main.setGameMode(OldGameMain.ModeID.kPlay);	
	}

	public override void doSelect()
	{
//		_main.setGameMode(OldGameMain.ModeID.kPlay);
	}
}

