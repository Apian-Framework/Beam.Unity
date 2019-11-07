using System.Collections.Generic;

namespace GameModeMgr 
{
	public interface IGameMode
	{			
		void Setup(ModeManager mgr, IGameInstance gameInst);
		void Start( object param = null);
		void Loop(float frameSecs);
		void Pause();
		void Resume(string prevModeName, object prevModeResult);
		object End();    
        void HandleCmd(int cmd, object param);
		string ModeName();
	};


	public abstract class BaseGameMode : IGameMode
	{	
		protected Dictionary<int,dynamic> _cmdDispatch;  	
		public ModeManager manager; 
		public IGameInstance gameInst;
			
		public void Setup(ModeManager mgr, IGameInstance gInst = null)
		{
			// Called by manager before Start()
			// Not virtual
			// TODO: this should be the engine and not the modeMgr - but what IS an engine...
 			_cmdDispatch = new Dictionary<int, dynamic>();
			manager = mgr;
			gameInst = gInst;

		}

		public virtual void Start( object param = null)	{}

		public virtual void Loop(float frameSecs) {}

		public virtual void Pause() {}
		public virtual void Resume(string prevModeName, object prevModeResult) {}	
		public virtual object End() { return null;}     

        public virtual void HandleCmd(int cmd, object param)
        {
            _cmdDispatch[cmd](param);            
        }	
        public virtual string ModeName() => this.GetType().Name;		   	
	};
}

