using System;
using System.Collections.Generic;
using UnityEngine;
using BeamGameCode;

public class BeamFeModeHelper : IFrontendModeHelper
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
    protected  BeamMain _beamMain;

    public BeamFeModeHelper(BeamMain beamMain)
    {
        _beamMain = beamMain;
        _modeFuncs = new Dictionary<int, ModeFuncs>()
        {
            { BeamModeFactory.kConnect, new ConnectModeFuncs(beamMain)},
            { BeamModeFactory.kSplash, new SplashModeFuncs(beamMain)},
            { BeamModeFactory.kPlay, new PlayModeFuncs(beamMain)},
            { BeamModeFactory.kPractice, new PracticeModeFuncs(beamMain)},
        };
    }

    private int _CurModeId() => _beamMain.beamApp.modeMgr.CurrentModeId();

    public void OnStartMode(int modeId, object parms=null)
    {
        Debug.Assert(_modeFuncs.ContainsKey(modeId));
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
        public override void OnStart(object parms=null)
        {
            //_feMain.frontend.startBtn.SetActive(false);
            _feMain.frontend.connectBtn.SetActive(false);
        }
        public override void OnEnd(object parms=null) {}
    }

    class SplashModeFuncs : ModeFuncs
    {
        public SplashModeFuncs(BeamMain bm) : base(bm)
        {
             _cmdDispatch[ModeSplash.kCmdTargetCamera] = new Action<object>(o => TargetCamera(o as TargetIdParams));
        }

        public override void OnStart(object parms=null)
        {
            SetupCamera();
            // TODO: These next should be in the GameUIController
            //_feMain.frontend.startBtn.SetActive(false);
            _feMain.frontend.connectBtn.SetActive(true);
        }

        public override void OnEnd(object parms=null)
        {

        }

        protected void TargetCamera(TargetIdParams parm)
        {
            GameObject tBike = _feMain.frontend.GetBikeObj(parm.targetId);
            _feMain.gameCamera.MoveCameraToTarget(tBike, 5f, 2f, .5f,  0); // Sets "close enough" value to zero - so it never gets there

            _feMain.gameCamera.StartBikeMode(tBike);

            // These just don;t look got ATM
            // int choice = (int)UnityEngine.Random.Range(0, 3 - .0001f);
            // switch (choice)
            // {
            //     case 0:
            //         _feMain.gameCamera.StartBikeMode(tBike);
            //         break;
            //     case 1:
            //         _feMain.gameCamera.StartOverheadMode(tBike);
            //         break;
            //     case 2:
            //         _feMain.gameCamera.StartOrbit(tBike, 20, new Vector3(1, 0, .5f) );
            //         break;
            // }

        }

        // protected GameObject GetRandomBikeObj()
        // {
        //     int index = UnityEngine.Random.Range(0, _feMain.frontend.BikeCount());
        //     return _feMain.frontend.GetBikeObjByIndex(index);
        // }

        protected void PointGameCamAtBike(string targetBikeId)
        {

            //_feMain.gameCamera.StartBikeMode(tBike);
            //_feMain.gameCamera.StartOrbit(tBike, 15f, new Vector3(0,3f,0));
        }

        protected void SetupCamera()
        {
            _feMain.gameCamera.transform.position = new Vector3(100, 100, 100);
		    _feMain.uiController.switchToNamedStage("SplashStage");
        }
    }

    class PlayModeFuncs : ModeFuncs
    {

        public PlayModeFuncs(BeamMain bm) : base(bm)
        {

        }

        public override void OnStart(object parms=null)
        {
            SetupCameras();
        }

        public override void OnEnd(object parms=null)
        {

        }

        protected void SetupCameras()
        {
            _feMain.gameCamera.transform.position = new Vector3(100, 100, 100);
            _feMain.uiController.switchToNamedStage("PlayStage");
        }


    }



class PracticeModeFuncs : PlayModeFuncs
    {
        public PracticeModeFuncs(BeamMain bm) : base(bm) {}
    }


}
