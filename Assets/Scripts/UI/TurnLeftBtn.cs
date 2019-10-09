using UnityEngine;
using System.Collections;

public class TurnLeftBtn : UIBtn  {
	
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
		// if (Input.GetKeyDown(KeyCode.LeftArrow))
		// 	_main.inputDispatch.LocalPlayerBikeLeft();		
	}

	public override void doSelect()
	{
		// _main.inputDispatch.LocalPlayerBikeLeft();
	}
}

