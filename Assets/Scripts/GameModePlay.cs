using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	


public class GameModePlay : GameMode
{		

	public override void init() 
	{
		base.init();
        GameMain mo = GameMain.GetInstance(); 
        mo.DestroyBikes();       
        GameObject bike = mo.CreateLocalPlayerBike(Color.yellow );
        _mainObj.gameCamera.StartBikeMode( bike);        
        mo.CreateDemoBike( Color.red );        
        mo.CreateDemoBike(Color.cyan );                
        mo.CreateDemoBike(Color.blue );             
 
		_mainObj.uiCamera.switchToNamedStage("PlayStage");
        _mainObj.gameCamera.gameObject.SetActive(true);   

	}
 
    public override void update()
    {
        
    }
        
    public override void HandleTap(bool isDown)     
    {
        //if (isDown == false)
        //    _mainObj.setGameMode(GameMain.ModeID.kSetupPlay);
    }
     
}
