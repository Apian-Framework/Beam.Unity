using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	

public class GameModeSplash : GameMode
{		
	
	public override void init() 
	{
		base.init();
        GameMain mo = GameMain.GetInstance();

        List<Bike> bikeList = new List<Bike>();

        foreach( Player p in DemoPlayers.data.Take(4)) 
        {
            bikeList.Add( BiekFactory.CreateDemoBike()  )
        }

        GameObject bike = mo.CreateDemoBike(Color.yellow );
        _mainObj.gameCamera.StartOrbit(bike, 20f, new Vector3(0,1,0));        
        mo.CreateDemoBike( Color.red );        
        mo.CreateDemoBike(Color.cyan );                
        mo.CreateDemoBike(Color.blue );             
 
		_mainObj.uiCamera.switchToNamedStage("SplashStage");
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
