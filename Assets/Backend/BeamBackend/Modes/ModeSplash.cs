using System;
using System.Linq;
using GameModeMgr;
using UnityEngine;

namespace BeamBackend
{
    // Remember, BaseGameMode is Setup() with:
    //  manager == ModeManager
	//	gameInst == GameInstance    
    public class ModeSplash : BaseGameMode
    {
        public class TargetIdParams {public string targetId;}        
        static public readonly int kCmdTargetCamera = 1;

	    static public readonly int kSplashBikeCount = 12;
        public BeamGameInstance game = null;      

		public override void Start(object param = null)	
        {
            base.Start();
            game = (BeamGameInstance)gameInst;
            game.DestroyPlayers();
            game.DestroyBikes();    
            game.ClearPlaces();     

            string cameraTargetBikeId = CreateADemoBike();
            for( int i=1;i<kSplashBikeCount; i++) 
                CreateADemoBike(); 

            game.frontend.ModeHelper()
                .OnStartMode(BeamModeFactory.kSplash, new TargetIdParams{targetId = cameraTargetBikeId} );             
        }

		public override void Loop(float frameSecs) {}

		public override object End() {
             return null;
        } 

        public override void HandleCmd(int cmd, object param)
        {

        }                

        protected string CreateADemoBike()
        {
            Player p = DemoPlayerData.CreatePlayer(); 
            game.NewPlayer(p);
            Heading heading = BikeFactory.PickRandomHeading();
            Vector2 pos = BikeFactory.PositionForNewBike( game.gameData.Bikes.Values.ToList(), heading, Ground.zeroPos, Ground.gridSize * 10 );
            string bikeId = Guid.NewGuid().ToString();
            IBike ib = BikeFactory.CreateBike(game, bikeId, p, BikeFactory.AiCtrl,pos, heading);
            game.NewBike(ib); 
            return ib.bikeId;          
        }

    }
}