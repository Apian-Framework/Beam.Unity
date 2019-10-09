using System;
using System.Collections.Generic;
using GameModeMgr;

namespace BeamBackend
{
    public class BeamGameData
    {
        public Dictionary<string, Player> Players { get; private set; } = null;
        public Dictionary<string, IBike> Bikes { get; private set; } = null;
	    public Ground Ground { get; private set; } = null;

        public BeamGameData(IFrontendProxy fep)
        {
            Players = new Dictionary<string, Player>();
            Bikes = new Dictionary<string, IBike>();
            Ground = new Ground(fep);              
        }

        public void Init() 
        {
            Players.Clear();
            Bikes.Clear();
        }

        public void Loop(float frameSecs)
        {
            Ground.Loop(frameSecs);
            foreach( IBike ib in Bikes.Values)
                ib.Loop(frameSecs);            
        }
    }

    public class BeamGameInstance : IGameInstance
    {
        public ModeManager modeMgr {get; private set;}
        public  BeamGameData gameData {get; private set;}
        public  IFrontendProxy feProxy {get; private set;}

        public BeamGameInstance(IFrontendProxy fep = null)
        {
            modeMgr = new ModeManager(new BeamModeFactory(), this);
            feProxy = fep; // Should work without one
            gameData = new BeamGameData(feProxy);            
        }

        public void Start()
        {
            modeMgr.Start(BeamModeFactory.kSplash);
        }

        public bool Loop(float frameSecs)
        {
            UnityEngine.Debug.Log("Inst.Loop()");
            gameData.Loop(frameSecs);
            return modeMgr.Loop(frameSecs);
        }

        // Player-related
        public void NewPlayer(Player p)
        {
            gameData.Players[p.ID] = p;
            feProxy?.NewPlayer(p);
        }
        public void DestroyPlayers()
        {
            gameData.Players.Clear();
        }

        // Bike-related
        public void NewBike(IBike b)
        {
            UnityEngine.Debug.Log(string.Format("NEW BIKE. ID: {0}, Pos: {1}", b.bikeId, b.position));            
            gameData.Bikes[b.bikeId] = b;
            feProxy?.NewBike(b);
        }        
        public void DestroyBikes()
        {
            feProxy?.DestroyBikes();
            gameData.Bikes.Clear();
        }

       // Ground-related
        public void ClearPlaces()
        {
            gameData.Ground.ClearPlaces();
        }

    }
}