using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeamGameCode;
using UniLog;

public class BeamFrontend : MonoBehaviour, IBeamFrontend
{
    public const float kErrorToastSecs = 5.0f;
    public const float kWarningToastSecs = 5.0f;

	public FeGround feGround;
    public GameObject connectBtn;
    public const string kSettingsFileBaseName = "unitybeamsettings";
    protected Dictionary<string, GameObject> feBikes;
    protected BeamMain mainObj; // main Unity GameObject
    public IBeamApplication beamAppl {get; private set;}
    public IBeamAppCore appCore {get; private set;}

    protected BeamUserSettings userSettings;
    protected BeamFeModeHelper _feModeHelper;
    public UniLogger logger;

    // Start is called before the first frame update
    void Start()
    {
        userSettings = UserSettingsMgr.Load(kSettingsFileBaseName);
        userSettings.localPlayerCtrlType = BikeFactory.LocalPlayerCtrl; // Kinda hackly

        mainObj = BeamMain.GetInstance();
        mainObj.ApplyUserSettings();
        mainObj.PersistSettings(); // make sure the default settings get saved
        _feModeHelper = new BeamFeModeHelper(mainObj);
        feBikes = new Dictionary<string, GameObject>();
        logger = UniLogger.GetLogger("Frontend");
    }

    public void SetBeamApplication(IBeamApplication appl)
    {
        beamAppl = appl;
    }

    public void SetAppCore(IBeamAppCore core)
    {
        appCore = core;
        if (core == null)
            return;

        OnNewCoreState(null, appCore.CoreData);

        appCore.NewCoreStateEvt += OnNewCoreState;
        appCore.PlayerJoinedEvt += OnPlayerJoinedEvt;
        appCore.PlayersClearedEvt += OnPlayersClearedEvt;
        appCore.NewBikeEvt += OnNewBikeEvt;
        appCore.BikeRemovedEvt += OnBikeRemovedEvt;
        appCore.BikesClearedEvt +=OnBikesClearedEvt;
        appCore.PlaceClaimedEvt += OnPlaceClaimedEvt;
        appCore.PlaceHitEvt += OnPlaceHitEvt;
        appCore.ReadyToPlayEvt += OnReadyToPlay;

    }

	public  int BikeCount() => feBikes.Count;

    public GameObject GetBikeObj(string bikeId)
    {
        try {
            return feBikes[bikeId];
        } catch (KeyNotFoundException) {
            return null;
        }
    }

    public List<GameObject> GetBikeList()
    {
        return feBikes.Values.ToList();
    }

    public GameObject GetBikeObjByIndex(int idx)
    {
        return feBikes.Values.ElementAt(idx);
    }

    public bool BikeIsLocal(IBike ib)
    {
        return ib.peerId == appCore.LocalPeerId;
    }
    public bool BikeIsLocalPlayer(IBike ib)
    {
        return (BikeIsLocal(ib) && ib.ctrlType == BikeFactory.LocalPlayerCtrl);
    }

    //
    // IBeamFrontend API
    //

    public BeamUserSettings GetUserSettings() => userSettings;

    public void DisplayMessage(MessageSeverity lvl, string msgText)
    {
        Toast.ToastColor color =  Toast.ToastColor.kBlue;
        string lvlStr = "Info";
        float secs = ToastMgr.defDisplaySecs;

        switch (lvl)
        {
            case MessageSeverity.Warning:
                color = Toast.ToastColor.kOrange;
                lvlStr =  "Warning";
                secs = kWarningToastSecs;
                break;
           case MessageSeverity.Error:
                color = Toast.ToastColor.kRed;
                lvlStr =  "Error";
                secs = kErrorToastSecs;
                break;
        }


        mainObj.uiController.ShowToast($"{lvlStr}: {msgText}", color, secs);
    }

    public void OnStartMode(int modeId, object param) =>  _feModeHelper.OnStartMode(modeId, param);
    public void OnEndMode(int modeId, object param) => _feModeHelper.OnEndMode(modeId, param);
    public void DispatchModeCmd(int modeId, int cmdId, object param) => _feModeHelper.DispatchCmd(modeId, cmdId, param);

    public void OnNewCoreState(object sender, BeamCoreState newCoreState)
    {
        newCoreState.PlaceFreedEvt += OnPlaceFreedEvt;
        newCoreState.PlacesClearedEvt += OnPlacesClearedEvt;
        newCoreState.SetupPlaceMarkerEvt += OnSetupPlaceMarkerEvt;
    }

    public void SelectGame( IDictionary<string, BeamGameInfo> existingGames )
    {
        mainObj.uiController.CurrentStage().transform.Find("SelGamePanel")?.SendMessage("LoadAndShow", existingGames);
    }

    public void OnGameSelected(BeamGameInfo selGame, GameSelectedArgs.ReturnCode result )
    {
        // TODO: should UI element (SelGamePanel) be calling beamApp directly? (I don't think so)

        if (result == GameSelectedArgs.ReturnCode.kCancel)
            mainObj.beamApp.OnSwitchModeReq(BeamModeFactory.kSplash, null);
        else
        {
            logger.Info($"OnGameSelected(): {selGame.GameName} : {result}");
            mainObj.beamApp.OnGameSelected(selGame, result);
        }
    }

    // Players

    public void OnPeerJoinedGameEvt(object sender, PeerJoinedArgs args)
    {
    //      BeamPeer p = args.peer;
    //      logger.Info($"New Peer: {p.Name}, Id: {p.PeerId}");
    }

    public void OnPeerLeftGameEvt(object sender, PeerLeftArgs args)
    {
         logger.Info("Peer Left: {args.p2pId}");
    }

    public void OnPlayerJoinedEvt(object sender, PlayerJoinedArgs args)
    {
        // Player joined means a group has been joined AND is synced (ready to go)
        if ( args.player.PeerId == appCore.LocalPeerId )
        {
            if (mainObj.beamApp.modeMgr.CurrentModeId() == BeamModeFactory.kPlay)
                mainObj.uiController.ShowToast($"GameSpec: {args.groupChannel}", Toast.ToastColor.kBlue, 10.0f);
        }
    }



    public void OnPlayersClearedEvt(object sender, EventArgs e)
    {
        logger.Info("OnPlayersClearedEvt() currently does nothing");
    }

    // Bikes

    public void OnNewBikeEvt(object sender, IBike ib)
    {
        bool isLocal = BikeIsLocal(ib);
        logger.Info($"OnNewBikeEvt(). Id: {ib.bikeId}, LocalPlayer: {(BikeIsLocalPlayer(ib))}");
        GameObject bikeGo = FrontendBikeFactory.CreateBike(ib, feGround, isLocal);
        feBikes[ib.bikeId] = bikeGo;
        if (BikeIsLocalPlayer(ib))
        {
            mainObj.inputDispatch.SetLocalPlayerBike(bikeGo);
            mainObj.uiController.CurrentStage().transform.Find("RestartBtn")?.SendMessage("moveOffScreen", null);
            mainObj.uiController.CurrentStage().transform.Find("Scoreboard")?.SendMessage("SetLocalPlayerBike", bikeGo);
            mainObj.gameCamera.StartBikeMode(bikeGo);
        }
        else
            mainObj.uiController.CurrentStage().transform.Find("Scoreboard")?.SendMessage("AddBike", bikeGo);

        mainObj.uiController.ShowToast($"New Bike: {ib.name}", Toast.ToastColor.kBlue);
    }

    public void OnBikeRemovedEvt(object sender, BikeRemovedData rData)
    {
        GameObject go = GetBikeObj(rData.bikeId);
        if (go == null)
            return;

        logger.Verbose($"OnBikeRemovedEvt() BikeId: {rData.bikeId}");
        IBike ib = appCore.CoreData.GetBaseBike(rData.bikeId);
        feBikes.Remove(rData.bikeId);
        mainObj.uiController.CurrentStage().transform.Find("Scoreboard")?.SendMessage("RemoveBike", go);
        if (BikeIsLocalPlayer(ib))
		{
		 	logger.Info("Boom! Local Player");
		 	mainObj.uiController.CurrentStage().transform.Find("RestartBtn")?.SendMessage("moveOnScreen", null);
		}
        mainObj.uiController.ShowToast($"{ib.name} Destroyed!!!", Toast.ToastColor.kOrange);
		GameObject.Instantiate(mainObj.boomPrefab, go.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(go);
    }
    //public void OnClearBikes(int modeId)
    public void OnBikesClearedEvt(object sender, EventArgs e)
    {
		foreach (GameObject bk in feBikes.Values)
		{
			GameObject.Destroy(bk);
		}
		feBikes.Clear();
    }

    public void OnPlaceHitEvt(object sender, PlaceHitArgs args)
    {
        GetBikeObj(args.ib.bikeId)?.GetComponent<FrontendBike>()?.OnPlaceHit(args.p);
    }
    public void OnPlaceClaimedEvt(object sender, BeamPlace p)
    {
        GetBikeObj(p.bike.bikeId)?.GetComponent<FrontendBike>()?.OnPlaceClaimed(p);
    }

    // Ground
    public void OnSetupPlaceMarkerEvt(object sender, BeamPlace p)
    {
        feGround.SetupMarkerForPlace(p);
    }
    //public void OnFreePlace(BeamPlace p, int modeId)
    public void OnPlaceFreedEvt(object sender, BeamPlace p)
    {
        logger.Debug($"OnPlaceFreedEvt() Placehash: {p.PosHash}");
        feGround.FreePlaceMarker(p);
    }
    //public void OnClearPlaces(int modeId)
    public void OnPlacesClearedEvt(object sender, EventArgs e)
    {
        feGround.ClearMarkers();
    }

    public void OnReadyToPlay(object sender, EventArgs e)
    {
        logger.Error($"OnReadyToPlay() - doesn't work anymore");
        //startBtn.SetActive(true);
        //mainObj.core.OnSwitchModeReq(BeamModeFactory.kPlay, null);
    }

}
