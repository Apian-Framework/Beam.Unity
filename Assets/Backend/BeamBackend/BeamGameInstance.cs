using System.Collections.Generic;
using GameModeMgr;

namespace BeamBackend
{
    public class BeamGameData
    {
        public Dictionary<string, Player> Players { get; private set; } = null;
        public Dictionary<string, BaseBike> Bikes { get; private set; } = null;

        public BeamGameData()
        {
            Players = new Dictionary<string, Player>();
            Bikes = new Dictionary<string, BaseBike>();                 
        }
    }

    public class BeamGameInstance : IGameInstance
    {
        protected ModeManager _modeMgr;
        protected BeamGameData _data;

        public BeamGameInstance()
        {
            _modeMgr = new ModeManager(new BeamModeFactory());
            _data = new BeamGameData();
        }

        public void Start()
        {
            _modeMgr.Start(BeamModeFactory.kStartup);
        }

        public bool Loop(float frameSecs)
        {
            return _modeMgr.Loop(frameSecs);
        }

    }
}