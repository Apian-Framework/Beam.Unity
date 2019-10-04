using UnityEngine;
using System.Collections;

public class TurnRightBtn : UIBtn  {
	
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
		if (Input.GetKeyDown(KeyCode.RightArrow))
			_main.inputDispatch.LocalPlayerBikeRight();		
	}

	public override void doSelect()
	{
		_main.inputDispatch.LocalPlayerBikeRight();
	}
}

