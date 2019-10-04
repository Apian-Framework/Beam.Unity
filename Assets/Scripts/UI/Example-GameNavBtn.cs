using UnityEngine;
using System.Collections;

public class GameNavBtn : UIBtn  {
	
	public enum Direction
	{
		kPrev,
		kNext,
		kBack,
		kUp,
		kDown
	};
		
	
	public Direction navDirection;
	
	protected OldGameMain _main = null;
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();		
	
		_main = (OldGameMain)utils.findObjectComponent("GameMain", "GameMain");
		
	}
		
	public override void doSelect()
	{
		//_main.doNavBtn(navDirection);
	}
}

