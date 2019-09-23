using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
	


public class GameModePlay : GameMode
{		

    public enum Commands
    {
        kInit = 0,
        kRespawn = 1,
        kCount = 2
    }    
    public readonly int kMaxPlayers = 12;

	public override void init() 
	{
		base.init();

        _cmdDispatch[(int)Commands.kInit] = new Action<object>( (o) => {} );  // TODO: &&&& First command invoke causes a delay "blip".  This is a bad answer.
        _cmdDispatch[(int)Commands.kRespawn] = new Action<object>(o => RespawnPlayerBike());

        _mainObj.backend.ClearPlayers();
        _mainObj.DestroyBikes();
        _mainObj.backend.ClearBikes();           
        _mainObj.ground.ClearPlaces();        

        // Create player bike
        GameObject playerBike = SpawnPlayerBike();

        for( int i=1;i<kMaxPlayers; i++) 
        {
            Player p = null;
            while (p == null) {
                p = DemoPlayerData.CreatePlayer();
                if (!_mainObj.backend.AddPlayer(p))
                    p = null;
            }
            SpawnAIBike(p); 
        }

        // Focus on first object
        _mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);               
		_mainObj.uiCamera.switchToNamedStage("PlayStage");   
        _mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard").SendMessage("SetLocalPlayerBike", playerBike); 
        foreach (GameObject b in _mainObj.BikeList.Values)
        {
            if (b != playerBike)
                _mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard").SendMessage("AddBike", b);
        }                         
        _mainObj.gameCamera.StartBikeMode( playerBike);        
        _mainObj.gameCamera.gameObject.SetActive(true);  
	}

    public override void handleCmd(int cmd, object param)
    {
        _cmdDispatch[cmd](param);            
    }

    protected BaseBike CreateBaseBike(Player p)
    {
        Heading heading = BikeFactory.PickRandomHeading();
        Vector3 pos = BikeFactory.PositionForNewBike( _mainObj.BikeList.Values.ToList(), heading, Ground.zeroPos, Ground.gridSize * 10 );   
        string bikeId = Guid.NewGuid().ToString();
         BaseBike bb = new BaseBike(_mainObj.backend, bikeId, p, pos, heading);
         p.Score = Player.kStartScore;
         _mainObj.backend.AddBike(bb); 
         return bb;
    }

    protected GameObject SpawnPlayerBike(Player p = null)
    {
        // Create one the first time
        while (p == null) {
            p = DemoPlayerData.CreatePlayer(true); 
            if (_mainObj.backend.AddPlayer(p) == false)
                p = null;
        }
           
        BaseBike bb = CreateBaseBike(p);  
        GameObject playerBike =  BikeFactory.CreateLocalPlayerBike(bb, _mainObj.ground);
        _mainObj.BikeList.Add(bb.bikeId, playerBike);   
        _mainObj.inputDispatch.SetLocalPlayerBike(playerBike);              
        return playerBike;
    }

    protected GameObject SpawnAIBike(Player p)
    {
        BaseBike bb = CreateBaseBike(p);  
        GameObject bike =  BikeFactory.CreateAIBike(bb, _mainObj.ground);
        _mainObj.BikeList.Add(bb.bikeId, bike);
        return bike;
    }


    public void RespawnPlayerBike()
    {       
        Player localPlayer = _mainObj.backend.Players.Values.Where( p => p.IsLocal).First();
        GameObject playerBike = SpawnPlayerBike(localPlayer);
        _mainObj.uiCamera.CurrentStage().transform.Find("RestartCtrl")?.SendMessage("moveOffScreen", null);         
        _mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard").SendMessage("SetLocalPlayerBike", playerBike); 
       _mainObj.gameCamera.StartBikeMode( playerBike);                
    }

    protected void RespawnAIBike(Player p)
    {
        GameObject bike = SpawnAIBike(p);
        _mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard").SendMessage("AddBike", bike);        
    }

    public override void update()
    {
        // TODO: clean this up
        List<GameObject> delBikes = new List<GameObject>();
        foreach ( GameObject go in _mainObj.BikeList.Values)
        {
            if (go.transform.GetComponent<Bike>().player.Score <=0)
                delBikes.Add(go);            
        }
        foreach ( GameObject go in delBikes) 
        {         
            _mainObj.RemoveOneBike(go);        
        }

        // Idle player to respawn?
        // TODO: add a delay?
        Player idle = FindIdlePlayer();
        if (idle != null)
        {
			Debug.Log(string.Format("Respawning AI: {0}", idle.ScreenName));
            RespawnAIBike(idle);
        }

    }
        
    protected Player FindIdlePlayer()
    {
        // TODO: yuk
        return _mainObj.backend.Players.Values.Where( (p) => !p.IsLocal && p.Score <= 0).FirstOrDefault();
    }

    public override void HandleTap(bool isDown)     
    {
        //if (isDown == false)
        //    _mainObj.setGameMode(GameMain.ModeID.kSetupPlay);
    }
     
}
