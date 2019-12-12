using System;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class BeamModeHelper : IFrontendModeHelper
{
 

    protected abstract class ModeFuncs
    {      
        public abstract void OnStart(object parms);
        public abstract void OnEnd(object parms);
        public void HandleCmd(int cmdId, object parms) => _cmdDispatch[cmdId](parms); 
        protected Dictionary<int,dynamic> _cmdDispatch;   
        protected BeamMain _feMain; // gives access to BemFrontend and IBeamBackend

        public ModeFuncs(BeamMain fe)
        {
            _feMain = fe;
            _cmdDispatch = new Dictionary<int, dynamic>();          
        }       
    }



    protected Dictionary<int, ModeFuncs> _modeFuncs;

    public BeamModeHelper(BeamMain beamMain) 
    {
        _modeFuncs = new Dictionary<int, ModeFuncs>()
        {
            { BeamModeFactory.kConnect, new ConnectModeFuncs(beamMain)},            
            { BeamModeFactory.kSplash, new SplashModeFuncs(beamMain)},
            { BeamModeFactory.kPlay, new PlayModeFuncs(beamMain)}            
        };
    }

    public void OnStartMode(int modeId, object parms=null)
    {
        _modeFuncs[modeId].OnStart(parms);
    }
    public void DispatchCmd(int modeId, int cmdId, object parms=null) 
    {
        _modeFuncs[modeId].HandleCmd(cmdId, parms);     
    } 
    public void OnEndMode(int modeId, object parms=null)
    {
        _modeFuncs[modeId].OnEnd(parms);
    }

    // Implementations
    class ConnectModeFuncs : ModeFuncs
    {
        public ConnectModeFuncs(BeamMain bm) : base(bm) {}
        public override void OnStart(object parms=null) {}      
        public override void OnEnd(object parms=null) {}       
    }    
    class SplashModeFuncs : ModeFuncs
    {
        public SplashModeFuncs(BeamMain bm) : base(bm)
        {
 //           _cmdDispatch[ModeSplash.kCmdTargetCamera] = new Action<object>(o => TargetCamera(o));            
        }

        // protected void TargetCamera(ModeSplash.TargetIdParams parm)
        // {

        // }

        public override void OnStart(object parms=null)
        {
            TargetIdParams p = (TargetIdParams)parms;
            SetupCameras(p.targetId);
        }
       
        public override void OnEnd(object parms=null)
        {

        }       

        protected GameObject GetRandomBikeObj()
        {
            int index = UnityEngine.Random.Range(0, _feMain.frontend.BikeCount()); 
            return _feMain.frontend.GetBikeObjByIndex(index);
        }

        protected void PointGameCamAtBike(string targetBikeId)
        {
            GameObject tBike = _feMain.frontend.GetBikeObj(targetBikeId);

            _feMain.gameCamera.transform.position = new Vector3(100, 100, 100);
            _feMain.gameCamera.MoveCameraToTarget(tBike, 5f, 2f, .5f,  0); // Sets "close enough" value to zero - so it never gets there
            //_feMain.gameCamera.StartBikeMode(tBike);  
            //_feMain.gameCamera.StartOrbit(tBike, 15f, new Vector3(0,3f,0));            
        }

        protected void SetupCameras(string targetBikeId)
        {
            PointGameCamAtBike(targetBikeId);
		    _feMain.uiCamera.switchToNamedStage("SplashStage");
            _feMain.gameCamera.gameObject.SetActive(true);               
        }        
    }

    class PlayModeFuncs : ModeFuncs
    {
        GameObject playerBikeGO = null;

        public PlayModeFuncs(BeamMain bm) : base(bm)
        {
 //           _cmdDispatch[ModeSplash.kCmdTargetCamera] = new Action<object>(o => TargetCamera(o));            
        }

        // protected void TargetCamera(ModeSplash.TargetIdParams parm)
        // {

        // }

        public override void OnStart(object parms=null)
        {
            TargetIdParams p = (TargetIdParams)parms;
            playerBikeGO = _feMain.frontend.GetBikeObj(p.targetId);
            SetupCameras(playerBikeGO);
        }
       
        public override void OnEnd(object parms=null)
        {

        }       

        protected void SetupCameras(GameObject playerBike)
        {
            _feMain.gameCamera.transform.position = new Vector3(100, 100, 100);               
            _feMain.uiCamera.switchToNamedStage("PlayStage");   
            _feMain.uiCamera.CurrentStage().transform.Find("Scoreboard").SendMessage("SetLocalPlayerBike", playerBike); 
            foreach (GameObject b in _feMain.frontend.GetBikeList())
            {
                if (b != playerBike)
                    _feMain.uiCamera.CurrentStage().transform.Find("Scoreboard").SendMessage("AddBike", b);
            }                         
            _feMain.gameCamera.StartBikeMode( playerBike);        
            _feMain.gameCamera.gameObject.SetActive(true);              
        }        
    }     


}
