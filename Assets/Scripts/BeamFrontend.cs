using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using UnityEngine;
using Apian;
using BeamGameCode;
using UniLog;
using static UniLog.UniLogger; // for SID

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

#if UNITY_WEBGL
        UniLogger.GetLogger("Apian").LogLevel = UniLogger.Level.Verbose;
        UniLogger.GetLogger("ApianClock").LogLevel = UniLogger.Level.Verbose;
        UniLogger.GetLogger("ApianGroup").LogLevel = UniLogger.Level.Verbose;
        UniLogger.GetLogger("ApianGroupSynchronizer").LogLevel = UniLogger.Level.Verbose;
        UniLogger.GetLogger("AppCore").LogLevel = UniLogger.Level.Debug;
        UniLogger.GetLogger("BaseBike").LogLevel = UniLogger.Level.Verbose;
        UniLogger.GetLogger("BeamApplication").LogLevel = UniLogger.Level.Debug;
        UniLogger.GetLogger("BeamMode").LogLevel = UniLogger.Level.Debug;
        UniLogger.GetLogger("BikeCtrl").LogLevel = UniLogger.Level.Verbose;
        UniLogger.GetLogger("CoreState").LogLevel = UniLogger.Level.Debug;
        UniLogger.GetLogger("Frontend").LogLevel = UniLogger.Level.Debug;
        UniLogger.GetLogger("GameInstance").LogLevel = UniLogger.Level.Debug;
        UniLogger.GetLogger("GameNet").LogLevel = UniLogger.Level.Debug;
        UniLogger.GetLogger("P2pNet").LogLevel = UniLogger.Level.Debug;
        UniLogger.GetLogger("P2pNetSync").LogLevel = UniLogger.Level.Verbose;
        UniLogger.GetLogger("UserSettings").LogLevel = UniLogger.Level.Debug;
// #else

#endif

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


        OnNewCoreState(null, new NewCoreStateEventArgs(core.CoreState)); // initialize

        appCore.NewCoreStateEvt += OnNewCoreState;
        appCore.PlayerJoinedEvt += OnPlayerJoinedEvt;
        appCore.PlayerMissingEvt += OnPlayerMissingEvt;
        appCore.PlayerReturnedEvt += OnPlayerReturnedEvt;
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

    protected void _SetupNewCorePlaces(BeamCoreState newCoreState)
    {
        // for each iBike, get a time-ordered list of the owned places
        foreach (IBike ib in newCoreState.Bikes.Values)
        {
            List<BeamPlace> places = newCoreState.PlacesForBike(ib).OrderBy( p => p.expirationTimeMs).ToList(); //ascending - oldest first
            BeamPlace prevBikePlace = null;
            foreach (BeamPlace p in places)
            {
                feGround.SetupMarkerForPlace(p);
                if (prevBikePlace != null && BeamPlace.AreAdjacent(prevBikePlace, p))
                {
                    feGround.SetupConnector(prevBikePlace, p);
                }
                prevBikePlace = p;
            }
        }

    }

    public void OnNewCoreState(object sender, NewCoreStateEventArgs e)
    {
        BeamCoreState newCoreState = e.coreState as BeamCoreState;
        _SetupNewCorePlaces(newCoreState);

        newCoreState.PlaceFreedEvt += OnPlaceFreedEvt;
        newCoreState.PlacesClearedEvt += OnPlacesClearedEvt;
    }


    public async Task<GameSelectedEventArgs> SelectGameAsync(IDictionary<string, BeamGameAnnounceData> existingGames)
    {
        TaskCompletionSource<GameSelectedEventArgs> tcs = new TaskCompletionSource<GameSelectedEventArgs>();

        logger.Info($"SelectGameAsync(): displaying UI");
        GameObject panelGo = mainObj.uiController.CurrentStage().transform.Find("SelGamePanel").gameObject;
        SelGamePanel panel = panelGo.GetComponent<SelGamePanel>();
        panel.LoadAndShow(existingGames, tcs);

        logger.Info($"SelectGameAsync(): awaiting task");
        return await tcs.Task;
    }

    public void OnGameSelected(GameSelectedEventArgs selection, TaskCompletionSource<GameSelectedEventArgs> tcs )
    {
        logger.Info($"OnGameSelected(): Setting result: {selection.gameInfo?.GameName} / {selection.result}");
        tcs.TrySetResult(selection);
    }

    // Players

    public void OnPeerJoinedGameEvt(object sender, PeerJoinedEventArgs args)
    {
    //      BeamPeer p = args.peer;
    //      logger.Info($"New Peer: {p.Name}, Id: {p.PeerId}");
    }

    public void OnPeerLeftGameEvt(object sender, PeerLeftEventArgs args)
    {
        logger.Info("Peer Left: {SID(args.p2pId)}");
        BeamPlayer pl = appCore.CoreState.GetPlayer(args.p2pId);
        mainObj.uiController.ShowToast($"Player {(pl!=null?pl.Name:"<unk>")} Left Game", Toast.ToastColor.kRed,5);
    }

    public void OnPlayerJoinedEvt(object sender, PlayerJoinedEventArgs args)
    {
        // Player joined means a group has been joined AND is synced (ready to go)
        if ( args.player.PeerId == appCore.LocalPeerId )
        {
            if (mainObj.beamApp.modeMgr.CurrentModeId() == BeamModeFactory.kPlay)
                mainObj.uiController.ShowToast($"GameSpec: {args.groupChannel}", Toast.ToastColor.kBlue, 10.0f);
        }
    }

    public void OnPlayerMissingEvt(object sender, PlayerLeftEventArgs args)
    {
        BeamPlayer pl = appCore.CoreState.GetPlayer(args.p2pId);
        mainObj.uiController.ShowToast($"Player {(pl!=null?pl.Name:"<unk>")} Missing!!", Toast.ToastColor.kRed,8);
    }

    public void OnPlayerReturnedEvt(object sender, PlayerLeftEventArgs args)
    {
        BeamPlayer pl = appCore.CoreState.GetPlayer(args.p2pId);
        mainObj.uiController.ShowToast($"Player {(pl!=null?pl.Name:"<unk>")} Returned!!", Toast.ToastColor.kRed,8);
    }

    public void OnPlayersClearedEvt(object sender, EventArgs e)
    {
        logger.Info("OnPlayersClearedEvt() currently does nothing");
    }

    // Bikes

    public void OnNewBikeEvt(object sender,  BikeEventArgs args)
    {
        IBike ib = args?.ib;
        bool isLocal = BikeIsLocal(ib);
        logger.Info($"OnNewBikeEvt(). Id: {SID(ib.bikeId)}, LocalPlayer: {(BikeIsLocalPlayer(ib))}");
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

    public void OnBikeRemovedEvt(object sender, BikeRemovedEventArgs args)
    {
        GameObject go = GetBikeObj(args.bikeId);
        if (go == null)
            return;

        logger.Verbose($"OnBikeRemovedEvt() BikeId: {SID(args.bikeId)}");
        IBike ib = appCore.CoreState.GetBaseBike(args.bikeId);
        feBikes.Remove(args.bikeId);
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

    public void OnPlaceHitEvt(object sender, PlaceHitEventArgs args)
    {
        GetBikeObj(args.ib.bikeId)?.GetComponent<FrontendBike>()?.OnPlaceHit(args.p);
    }
    public void OnPlaceClaimedEvt(object sender, BeamPlaceEventArgs args)
    {
        BeamPlace p = args?.p;
        feGround.SetupMarkerForPlace(p);
        GetBikeObj(p.bike.bikeId)?.GetComponent<FrontendBike>()?.OnPlaceClaimed(p);
    }

    // Ground


    public void OnPlaceFreedEvt(object sender, BeamPlaceEventArgs args)
    {
        BeamPlace p = args.p;
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
