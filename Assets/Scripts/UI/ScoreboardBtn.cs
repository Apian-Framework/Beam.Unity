using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardBtn : UIBtn
{
	protected override void Start () 
	{
		base.Start();			
	}

  	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.S))
			transform.parent.SendMessage("toggle");	
	}

	public override void doSelect()
	{
		transform.parent.SendMessage("toggle");
	}
}
