using System.Collections.Generic;
using System.Linq;
using System;

namespace GameModeMgr 
{
	public interface IModeFactory
	{
		IGameMode Create(int modeId);		
	}

	public class ModeFactory : IModeFactory
	{
		protected Dictionary<int, Func<IGameMode>> modeFactories	= null;			
        public IGameMode Create(int modeId) 
        {
            return modeFactories[modeId]();
        }	
	};
}

