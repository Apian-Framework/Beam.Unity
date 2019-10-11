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
	    public readonly int kSplashBikeCount = 12;
        public BeamGameInstance game = null;
        public BeamMain mainObj = null;        

        protected string _cameraTargetBikeId;

		public override void Start( object param = null)	
        {
            base.Start();
            mainObj = BeamMain.GetInstance();
            game = (BeamGameInstance)gameInst;
            game.DestroyPlayers();
            game.DestroyBikes();    
            game.ClearPlaces();          

            _cameraTargetBikeId = CreateADemoBike();
            for( int i=1;i<kSplashBikeCount; i++) 
                CreateADemoBike();

            SetupCameras(_cameraTargetBikeId);
        }

		public override void Loop(float frameSecs) 
        {
            // &&&&jkb This doesn;t work as intended - the camera in the initial
            // zoom-to mode very seldom "gets there" - it's just a bug but I'm not gonna fix it now
            // TODO: consider computing the "offset" param from the current camera/bike location.
            if ( mainObj.gameCamera._curModeID == GameCamera.CamModeID.kNormal)
                mainObj.gameCamera.StartOrbit(mainObj.frontend.GetBikeObj(_cameraTargetBikeId), 20f, new Vector3(0,2,0));            
        }

		public override object End() {
             return null;
        } 

        public override void HandleCmd(int cmd, object param)
        {

        }                

        protected GameObject GetRandomBikeObj()
        {
            int index = UnityEngine.Random.Range(0, mainObj.frontend.BikeCount()); 
            return mainObj.frontend.GetBikeObjByIndex(index);
        }

        protected void PointGameCamAtBike(string targetBikeId)
        {
            GameObject tBike = mainObj.frontend.GetBikeObj(targetBikeId);

            mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);
            mainObj.gameCamera.MoveCameraToTarget(tBike, 5f, 2f, .5f,  .3f);
        }

        protected void SetupCameras(string targetBikeId)
        {
            PointGameCamAtBike(targetBikeId);
		    mainObj.uiCamera.switchToNamedStage("SplashStage");
            mainObj.gameCamera.gameObject.SetActive(true);               
        }

        protected string CreateADemoBike()
        {
            Player p = DemoPlayerData.CreatePlayer(); 
            game.NewPlayer(p);
            Heading heading = BikeFactory.PickRandomHeading();
            Vector2 pos = BikeFactory.PositionForNewBike( game.gameData.Bikes.Values.ToList(), heading, Ground.zeroPos, Ground.gridSize * 10 );
            string bikeId = Guid.NewGuid().ToString();
            IBike ib = BikeFactory.CreateBike(game.frontend, bikeId, p, BikeFactory.AiCtrl,pos, heading);
            game.NewBike(ib); 
            return ib.bikeId;          
        }

    }
}