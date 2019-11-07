using System;
using System.Collections.Generic;
using System.Linq;

namespace GameModeMgr 
{
	public class ModeManager {

		// Operations

		protected enum ModeOp
		{ 
			kNop = 0,
			kSwitch,
			kPush,
			kPop,
			kQuit
		};	
		protected class OpData 
		{
			public ModeOp nextOp = ModeOp.kNop;		
			public int nextModeId;
			public object nextParam = null;

			public static OpData DoNothing;
			public static OpData DoQuit;		

			public OpData(ModeOp op, int modeId, object param) 
			{
				nextOp = op; 
				nextModeId = modeId;
				nextParam = param;
			}

			static OpData()
			{
				DoNothing = new OpData(ModeOp.kNop, -1, null);
				DoQuit = new OpData(ModeOp.kQuit, -1, null);			
			}
		}

		protected class ModeData
		{
			public int modeId;
			public IGameMode mode;
			public ModeData(int mId, IGameMode m)
			{
				modeId = mId;
				mode = m;
			}
		}
	
		// Singleton access
		// TODO: do we REALLY want the singleton pattern?
		public static ModeManager instance {get; private set;} = null; 
		
		//
		// Lifecycle
		//

		protected IModeFactory _factory;
		protected IGameInstance _gameInst;
		protected Stack<ModeData> _modeDataStack = null;	
		protected OpData _nextOpData = null;

		public ModeManager(IModeFactory factory, IGameInstance gameInst = null) 
		{
			instance = this;
			_factory = factory;
			_modeDataStack = new Stack<ModeData>();
			_nextOpData = OpData.DoNothing;
			_gameInst = gameInst;
		} 

		public void Start(int startModeId, object startParam = null) { 
			_nextOpData = new OpData(ModeOp.kPush, startModeId, startParam);
		}
		
		public void Stop() 	
		{ 
			_nextOpData = OpData.DoQuit; 
		}	

		public void SwitchToMode( int newModeId, object param=null)
		{
			_nextOpData = new OpData(ModeOp.kSwitch, newModeId, param);		
		}

		public void PushMode( int newModeId, object param=null)
		{
			_nextOpData = new OpData(ModeOp.kPush, newModeId, param);		
		}

		public void PopMode(object result=null)
		{
			_nextOpData = new OpData(ModeOp.kPop, -1, result);		
		}

		public virtual bool Loop(float frameSecs) 
		{ 
			// return false to signal quit

			// Get current op data and reset instance var
			IGameMode orgMode = CurrentMode();
			OpData curOpData = _nextOpData;
			_nextOpData = OpData.DoNothing;

			// stop the current state and start/resume another
			switch (curOpData.nextOp)
			{
			case ModeOp.kQuit:
				_Stop();
				return false; //  short circuit exit

			case  ModeOp.kSwitch:
				_StopCurrentMode();
				_StartMode(curOpData);
				break;

			case ModeOp.kPop:
				string prevName = CurrentMode().GetType().Name;      
				_StopCurrentMode();
				_ResumeMode(prevName, curOpData.nextParam);
				break;

			case ModeOp.kPush:                   
				_SuspendCurrentMode();
				_StartMode(curOpData);
				break;

			}	

			//# Now - whatever is current, call its loop
			if (_modeDataStack.Count > 0)
				CurrentMode().Loop(frameSecs);
			
			// If nothing on stack - we're done
			return _modeDataStack.Count > 0;
		}	

		public virtual void DispatchCmd(int cmd, object param = null) 
		{
			CurrentMode()?.HandleCmd(cmd, param);  
		}

		public IGameMode CurrentMode()
		{
			return _CurrentModeData()?.mode;
		}

		public int CurrentModeId()
		{
			return _CurrentModeData()?.modeId ?? -1;
		}

		//
		// Internal calls
		// 
		protected ModeData _CurrentModeData()
		{
			try {
				return _modeDataStack.Peek();
			} catch (InvalidOperationException) {
				return null;
			}			
		}

		protected object _StopCurrentMode()
		{
			//  pop the current state from the stack and call it's end()
			// returns the result from end()
			// NOTE: nothing is currently done with the return val from end()
			object retVal = null;
			IGameMode oldMode = CurrentMode();
			if ( oldMode != null)
			{      
				retVal = oldMode.End(); // should still be on the stack (for potential GetCurrentState() during pop) TODO: Is this true?
				_modeDataStack.Pop();  
			}      
			return retVal;
		}
		protected void _Stop()
		{
			// Unwind the stack
			while(_modeDataStack.Count > 0)
				_StopCurrentMode();
		}

		protected void _StartMode(OpData opData)
		{
			IGameMode nextMode = _factory.Create(opData.nextModeId);
			_modeDataStack.Push(new ModeData(opData.nextModeId, nextMode));
			nextMode.Setup(this, _gameInst);
			nextMode.Start(opData.nextParam);
		}

		protected void _SuspendCurrentMode()
		{
			// Pause current state before pushing a new one - leave on stack
			CurrentMode()?.Pause();
		}

		protected void _ResumeMode(string prevStateName, object resultVal)
		{
			// Resume state a top of stack, passing it
			// the result of the state that ended
			CurrentMode()?.Resume(prevStateName, resultVal);	
		}
	}
}
