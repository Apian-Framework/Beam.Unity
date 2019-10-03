using GameModeMgr;

namespace BeamBackend
{
    public class ModeSplash : BaseGameMode
    {
	    public readonly int kSplashBikeCount = 12;
        public BeamGameInstance game = null;

		public override void Start( object param = null)	
        {
            base.Start();
            game = (BeamGameInstance)gameInst;
            game.DestroyPlayers();
            game.DestroyBikes();    
            game.ClearPlaces();
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


    }
}