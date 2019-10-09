using System;
using System.Linq;
using GameModeMgr;
using UnityEngine;

namespace BeamBackend
{
    // Remeber, BaseGameMode is Setup() with:
    // manager == ModeManager
	//	gameInst == GameInstance    
    public class ModeSplash : BaseGameMode
    {
	    public readonly int kSplashBikeCount = 3;
        public BeamGameInstance game = null;

		public override void Start( object param = null)	
        {
            base.Start();
            game = (BeamGameInstance)gameInst;
            game.DestroyPlayers();
            game.DestroyBikes();    
            game.ClearPlaces();

            UnityEngine.Debug.Log("Before CreateBikes");            
            CreateTheBikes();
            UnityEngine.Debug.Log("After CreateBikes");
        }

		public override void Loop(float frameSecs) 
        {
            
        }

		public override object End() {
             return null;
        } 

        public override void HandleCmd(int cmd, object param)
        {

        }                

        protected void CreateTheBikes()
        {
            for( int i=0;i<kSplashBikeCount; i++) 
            {
                Player p = DemoPlayerData.CreatePlayer(); 
                game.NewPlayer(p);

                Heading heading = BikeFactory.PickRandomHeading();
                Vector2 pos = BikeFactory.PositionForNewBike( game.gameData.Bikes.Values.ToList(), heading, Ground.zeroPos, Ground.gridSize * 10 );
                string bikeId = Guid.NewGuid().ToString();

                IBike ib = BikeFactory.CreateDemoBike(bikeId, p, pos, heading);
                game.NewBike(ib);           
            }
        }

    }
}