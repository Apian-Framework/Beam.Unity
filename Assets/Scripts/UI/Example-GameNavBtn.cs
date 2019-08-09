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
	
	protected GameMain _main = null;
	
	// Use this for initialization
	protected override void Start () 
	{
		base.Start();		
	
		_main = (GameMain)utils.findObjectComponent("GameMain", "GameMain");
		
	}
		
	public override void doSelect()
	{
		//_main.doNavBtn(navDirection);
	}
}

