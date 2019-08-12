using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
	


public class GameModePlay : GameMode
{		
    public readonly int kMaxPlayers = 3;

	public override void init() 
	{
		base.init();
        _mainObj.DestroyBikes();

        Assert.IsTrue(kMaxPlayers <= SplashPlayers.count, "Too many bikes for splash players list");

        // Create player bike
        Player p = SplashPlayers.data[0]; 
        Heading heading = BikeFactory.PickRandomHeading();
        Vector3 pos = BikeFactory.PositionForNewBike( _mainObj.BikeList, heading, Ground.zeroPos, Ground.gridSize * 5 );            
        GameObject playerBike =  BikeFactory.CreateLocalPlayerBike(p, _mainObj.ground, pos, heading);
        _mainObj.BikeList.Add(playerBike);   
        _mainObj.inputDispatch.SetLocalPlayerBike(playerBike);     

        for( int i=1;i<kMaxPlayers; i++) 
        {
            p = SplashPlayers.data[i]; 
		    heading = BikeFactory.PickRandomHeading();
		    pos = BikeFactory.PositionForNewBike( _mainObj.BikeList, heading, Ground.zeroPos, Ground.gridSize * 5 );            
            GameObject bike =  BikeFactory.CreateAIBike(p, _mainObj.ground, pos, heading);
            _mainObj.BikeList.Add(bike);
        }

        // Focus on first object
        _mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);
        //_mainObj.gameCamera.MoveCameraToTarget(playerBike, 5f, 2f, .5f,  .3f);                

 
		_mainObj.uiCamera.switchToNamedStage("PlayStage");
        _mainObj.gameCamera.StartBikeMode( playerBike);           
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
