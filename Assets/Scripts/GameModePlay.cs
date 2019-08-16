using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
	


public class GameModePlay : GameMode
{		
    public readonly int kMaxPlayers = 12;

	public override void init() 
	{
		base.init();
        _mainObj.DestroyBikes();
        _mainObj.ground.ClearPlaces();

        Assert.IsTrue(kMaxPlayers <= SplashPlayers.count, "Too many bikes for splash players list");

        // Create player bike
        Player p = SplashPlayers.data[0]; 
        Heading heading = BikeFactory.PickRandomHeading();
        Vector3 pos = BikeFactory.PositionForNewBike( _mainObj.BikeList, heading, Ground.zeroPos, Ground.gridSize * 10 );            
        GameObject playerBike =  BikeFactory.CreateLocalPlayerBike(p, _mainObj.ground, pos, heading);
        _mainObj.BikeList.Add(playerBike);   
        _mainObj.inputDispatch.SetLocalPlayerBike(playerBike);     

        for( int i=1;i<kMaxPlayers; i++) 
        {
            p = SplashPlayers.data[i]; 
		    heading = BikeFactory.PickRandomHeading();
		    pos = BikeFactory.PositionForNewBike( _mainObj.BikeList, heading, Ground.zeroPos, Ground.gridSize *  10 );            
            GameObject bike =  BikeFactory.CreateAIBike(p, _mainObj.ground, pos, heading);
            _mainObj.BikeList.Add(bike);
        }

        // Focus on first object
        _mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);
        //_mainObj.gameCamera.MoveCameraToTarget(playerBike, 5f, 2f, .5f,  .3f);                

 
		_mainObj.uiCamera.switchToNamedStage("PlayStage");
        _mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard").SendMessage("SetLocalPlayerBike", playerBike);
        foreach (GameObject b in _mainObj.BikeList)
        {
            if (b != playerBike)
                _mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard").SendMessage("AddBike", b);
        }

        _mainObj.gameCamera.StartBikeMode( playerBike);           
        _mainObj.gameCamera.gameObject.SetActive(true);  
 

	}
 
    public override void update()
    {
        // TODO: clean this up
        List<GameObject> delBikes = new List<GameObject>();
        foreach ( GameObject go in _mainObj.BikeList)
        {
            if (go.transform.GetComponent<Bike>().player.Score < 0)
                delBikes.Add(go);            
        }
        foreach ( GameObject go in delBikes)
            _mainObj.RemoveOneBike(go);        
    }
        
    public override void HandleTap(bool isDown)     
    {
        //if (isDown == false)
        //    _mainObj.setGameMode(GameMain.ModeID.kSetupPlay);
    }
     
}
