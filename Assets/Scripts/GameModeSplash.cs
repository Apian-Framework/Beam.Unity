using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
	

public class GameModeSplash : GameMode
{		
	public readonly int kSplashBikeCount = 4;
	public override void init() 
	{
		base.init();
        _mainObj.DestroyBikes();
        _mainObj.ground.ClearPlaces();

        Assert.IsTrue(kSplashBikeCount <= SplashPlayers.count, "Too many bikes for splash players list");

        for( int i=0;i<kSplashBikeCount; i++) 
        {
            Player p = SplashPlayers.data[i]; 
		    Heading heading = BikeFactory.PickRandomHeading();
		    Vector3 pos = BikeFactory.PositionForNewBike( _mainObj.BikeList, heading, Ground.zeroPos, Ground.gridSize * 5 );            
            GameObject bike =  BikeFactory.CreateDemoBike(p, _mainObj.ground, pos, heading);
            _mainObj.BikeList.Add(bike);
        }

        // Focus on first object
        _mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);
        _mainObj.gameCamera.MoveCameraToTarget(_mainObj.BikeList[0], 5f, 2f, .5f,  .3f);                

		_mainObj.uiCamera.switchToNamedStage("SplashStage");
        _mainObj.gameCamera.gameObject.SetActive(true);   

	}
 
    public override void update()
    {
        // &&&&jkb This doesn;t work as intended - the camera in the initial
        // zoom-to mode very seldom "gets there" - it's just a bug but I'm not gonna fix it now
        // TODO: consider computing the "offset" param from the current camera/bike location.
        if (_mainObj.gameCamera._curModeID == GameCamera.CamModeID.kNormal)
            _mainObj.gameCamera.StartOrbit(_mainObj.BikeList[0], 20f, new Vector3(0,2,0));    
    }
        
    public override void HandleTap(bool isDown)     
    {
        if (isDown == false)
            _mainObj.setGameMode(GameMain.ModeID.kPlay);
    }
}
