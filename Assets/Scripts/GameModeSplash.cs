using UnityEngine;
using System;
using System.Linq;
using BeamBackend;
	

public class GameModeSplash : GameMode
{	
	public readonly int kSplashBikeCount = 12;
	public override void init() 
	{
		base.init();
        _mainObj.backend.ClearPlayers();
        _mainObj.DestroyBikes();
        _mainObj.backend.ClearBikes();        
        _mainObj.ground.ClearPlaces();

        for( int i=0;i<kSplashBikeCount; i++) 
        {
            Player p = DemoPlayerData.CreatePlayer(); 
            _mainObj.backend.AddPlayer(p);

		    Heading heading = BikeFactory.PickRandomHeading();
		    Vector3 pos = BikeFactory.PositionForNewBike( _mainObj.BikeList.Values.ToList(), heading, Ground.zeroPos, Ground.gridSize * 10 );
            string bikeId = Guid.NewGuid().ToString();

            BaseBike bb = new BaseBike(_mainObj.backend, bikeId, p, pos, heading);
            _mainObj.backend.AddBike(bb);            

            GameObject bike =  BikeFactory.CreateDemoBike(bb, _mainObj.ground);
            _mainObj.BikeList.Add(bb.bikeId, bike);
        }

        // Focus on first object
        _mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);
        _mainObj.gameCamera.MoveCameraToTarget(_mainObj.BikeList.Values.First(), 5f, 2f, .5f,  .3f);                

		_mainObj.uiCamera.switchToNamedStage("SplashStage");
        _mainObj.gameCamera.gameObject.SetActive(true);   

	}
 
    public override void update()
    {
        // &&&&jkb This doesn;t work as intended - the camera in the initial
        // zoom-to mode very seldom "gets there" - it's just a bug but I'm not gonna fix it now
        // TODO: consider computing the "offset" param from the current camera/bike location.
        if (_mainObj.gameCamera._curModeID == GameCamera.CamModeID.kNormal)
            _mainObj.gameCamera.StartOrbit(_mainObj.BikeList.Values.First(), 20f, new Vector3(0,2,0));    
    }
        
    public override void HandleTap(bool isDown)     
    {
        if (isDown == false)
            _mainObj.setGameMode(GameMain.ModeID.kPlay);
    }
}
