using UnityEngine;
using System.Collections;


public class RestartBtn : UIBtn  {
	
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
// &&&&& Call BeamMain.Respawn() which will tell the backend to add a new bike for the local player
// Could ask the GameProxy directly, but this is just a button - it shouldnt know about the game
//		if (Input.GetKeyDown(KeyCode.Return))
//			_main.SendCmd((int)GameModePlay.Commands.kRespawn, null);	
	}

	public override void doSelect()
	{
// &&& See above		_main.SendCmd((int)GameModePlay.Commands.kRespawn, null);
	}
}

