using System.Collections.Generic;
using GameModeMgr;

namespace BeamBackend
{
    public class BeamGameData
    {
        public Dictionary<string, Player> Players { get; private set; } = null;
        public Dictionary<string, BaseBike> Bikes { get; private set; } = null;
	    public Ground Ground { get; private set; } = null;

        public BeamGameData(FrontendProxy fep)
        {
            Players = new Dictionary<string, Player>();
            Bikes = new Dictionary<string, BaseBike>();
            Ground = new Ground(fep);              
        }
    }

    public class BeamGameInstance : IGameInstance
    {
        protected ModeManager _modeMgr;
        protected BeamGameData _data;
        protected FrontendProxy _feProxy;

        public BeamGameInstance(FrontendProxy fep = null)
        {
            _modeMgr = new ModeManager(new BeamModeFactory());
            _feProxy = fep; // Should work without one
            _data = new BeamGameData(_feProxy);            
        }

        public void Start()
        {
            _modeMgr.Start(BeamModeFactory.kSplash);
        }

        public bool Loop(float frameSecs)
        {
            return _modeMgr.Loop(frameSecs);
        }

        // Player-related
        public void DestroyPlayers()
        {
            _data.Players.Clear();
        }

        // Bike-related
        public void DestroyBikes()
        {
            _feProxy?.DestroyBikes();
            _data.Bikes.Clear();
        }

       // Ground-related
        public void ClearPlaces()
        {
            _data.Ground.ClearPlaces();
        }

    }
}