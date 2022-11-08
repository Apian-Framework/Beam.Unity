using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Apian;
using BeamGameCode;
using UniLog;
using static UniLog.UniLogger; // for SID
using ApianCrypto;

#if !SINGLE_THREADED
using System.Threading;
using System.Threading.Tasks;
#endif

public class BeamFrontend : MonoBehaviour, IBeamFrontend
{
    public const float kErrorToastSecs = 7.0f;
    public const float kWarningToastSecs = 5.0f;
	public FeGround feGround;
    public GameObject connectBtn;
    public GameObject logLevelEditBtn;
    public NetworkStage networkStage;

    public const string kSettingsFileBaseName = "unitybeamsettings";
    protected Dictionary<string, GameObject> feBikes;
    protected BeamMain mainObj; // main Unity GameObject
    public IBeamApplication beamAppl {get; private set;}
    public IBeamAppCore appCore {get; private set;}
    public IApianCrypto cryptoThing {get; private set;}
    protected BeamUserSettings userSettings;


    protected string _startupErrorMsg;

    Dictionary<int, Action<BeamGameMode, object>> modeStartActions;
    Dictionary<int, Action<BeamGameMode, object>> modeEndActions;
    Dictionary<int, Action<BeamGameMode, object>> modeResumeActions;
    Dictionary<int, Action<BeamGameMode, object>> modePauseActions;


    public UniLogger logger;

#if UNITY_WEBGL
    void SetupWebGLLogging()
    {

        // TODO: come up with JSON file web support for this.
        //
        // UniLogger.GetLogger("Apian").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("ApianClock").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("ApianGroup").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("ApianGroupSynchronizer").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("AppCore").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("BaseBike").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("BeamApplication").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("BeamMode").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("BikeCtrl").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("CoreState").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("Frontend").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("GameInstance").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("GameNet").LogLevel = UniLogger.Level.Info;
        // UniLogger.GetLogger("P2pNet").LogLevel = UniLogger.Level.Verbose;
        // UniLogger.GetLogger("P2pNetSync").LogLevel = UniLogger.Level.Info;
        UniLogger.GetLogger("UserSettings").LogLevel = UniLogger.Level.Verbose;

        UniLogger.DefaultLevel = UniLogger.Level.Info;
    }
#endif

    // Awake
    void Awake()
    {
#if UNTIY_WEBGL
        SetupWebGLLogging();
#endif

        try {
            userSettings = UserSettingsMgr.Load(kSettingsFileBaseName);
        } catch (UserSettingsException ex) {
            _startupErrorMsg = $"WARNING: {ex.Message}. Using default settings.";
            userSettings = BeamUserSettings.CreateDefault();
        } catch (Exception ex) {
            _startupErrorMsg = $"LoadSettings failed: {ex.Message}. Default settings used.";
            userSettings = BeamUserSettings.CreateDefault();
        }

        UniLogger.DefaultLevel = UniLogger.LevelFromName(userSettings.defaultLogLevel);
        UniLogger.SetupLevels(userSettings.logLevels);
        userSettings.localPlayerCtrlType = BikeFactory.LocalPlayerCtrl; // FIXME: is this necessary?
    }

    // Start is called before the first frame update
    void Start()
    {
        mainObj = BeamMain.GetInstance();
        mainObj.ApplyPlatformUserSettings();
        mainObj.PersistSettings(); // make sure the default settings get saved
        feBikes = new Dictionary<string, GameObject>();
        logger = UniLogger.GetLogger("Frontend");
        SetupModeActions();

        mainObj.uiController.ClearToasts();

        cryptoThing = EthForApian.Create();

        if (string.IsNullOrEmpty(userSettings.cryptoAcctJSON))
        {
            string addr =  cryptoThing.CreateAccount();
            string json = cryptoThing.GetJsonForAccount("password");
            mainObj.uiController.ShowToast( $"Created new Eth acct: {addr}", Toast.ToastColor.kBlue, 5,  "acct");
            userSettings.cryptoAcctJSON = json;
            mainObj.PersistSettings();
        } else {
            string addr = cryptoThing.CreateAccountFromJson("password", userSettings.cryptoAcctJSON);
            mainObj.uiController.ShowToast(  $"Loaded Eth acct: {addr} from settings", Toast.ToastColor.kBlue, 5, "acct");
        }

#if ETH_SIGN_RECOVER_TEST
            // Stupid temporary test
        string msg = "Ya Ya! Ya Ya ya.";
        string sig = cryptoThing.EncodeUTF8AndSign(msg);

        logger.Info( $"Message: {msg}");
        logger.Info( $"Signature: {sig}");

        string recAddr = cryptoThing.EncodeUTF8AndEcRecover(msg, sig);
        logger.Info( $"Recovered addr: {recAddr}");

        if (recAddr.Equals(cryptoThing.AccountAddress))
            mainObj.uiController.ShowToast($"Lame Sign/Recover test succeeded", Toast.ToastColor.kGreen, 20, "test");
        else
           mainObj.uiController.ShowToast($"Lame Sign/Recover test FAILED!", Toast.ToastColor.kRed, 20, "test");
#endif

        if (_startupErrorMsg != null) {
            mainObj.uiController.ShowToast($"Startup Error: {_startupErrorMsg}", Toast.ToastColor.kRed, 10, "crashTag");
        }

    }

    public void EnableLogLevelBtn(bool bDoIt)
    {
        // platform setting
        logLevelEditBtn.SetActive(bDoIt);
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


    private readonly Dictionary<ApianGroupMember.Status, string> statusNames = new Dictionary<ApianGroupMember.Status, string>{
        {ApianGroupMember.Status.New, "New"},
        {ApianGroupMember.Status.Joining, "Joining"},
        {ApianGroupMember.Status.SyncingState, "SyncingState"},
        {ApianGroupMember.Status.SyncingClock, "SyncingClock"},
        {ApianGroupMember.Status.Active, "Active"},
        {ApianGroupMember.Status.Gone, "Gone"}
    };

    public void OnGroupMemberStatus(string groupId, string peerId, ApianGroupMember.Status newStatus, ApianGroupMember.Status prevStatus)
    {
        if ((newStatus != prevStatus) && (peerId == appCore?.LocalPeerId ))
        {
            mainObj.uiController.ShowToast($"Local Peer is {statusNames[newStatus]}", Toast.ToastColor.kGreen, 5, "peerStatusTag");
         }

    }

    public void OnGroupLeaderChanged(string groupId, string newLeaderId, string leaderName)
    {
        string msg = "Group Leader: " +  (leaderName != null ? $"{leaderName} {SID(newLeaderId)}"  : SID(newLeaderId));

        mainObj.uiController.ShowToast(msg, Toast.ToastColor.kOrange, 5, "groupLeadTag");

    }

    public void UpdateNetworkInfo()
    {
        BeamNetInfo netInfo = beamAppl.NetInfo;
        logger.Info($"** Network Info: Name: {netInfo.NetName}, Peers: {netInfo.PeerCount}, Games: {netInfo.GameCount}");

        networkStage.OnNetUpdate(netInfo);
    }

    public void OnNetworkReady()
    {
        logger.Info($"OnNetworkReady() ");

        networkStage.ShowProceedButton();

        // Need to display "Proceed" and "Cancel" (no, tnot hose prompts) to the user
        // that result in a call to:
        //     beamAppl.OnPushModeReq(BeamModeFactory.kNetPlay, null);
        // or
        //     beamAppl.OnSwitchModeReq(BeamModeFactory.kNetSplash, null);

        // throw new NotImplementedException();
    }

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

   //
   // Backend game modes
   //
    protected void SetupModeActions()
    {
        modeStartActions = new Dictionary<int, Action<BeamGameMode, object>>()
        {
            { BeamModeFactory.kSplash, OnStartSplash},
            { BeamModeFactory.kPractice, OnStartPractice},
            { BeamModeFactory.kNetwork, OnStartNetworkMode},
            { BeamModeFactory.kNetPlay, OnStartNetPlay},
        };

        modeEndActions = new Dictionary<int, Action<BeamGameMode, object>>()
        {
            { BeamModeFactory.kSplash, OnEndSplash},
            { BeamModeFactory.kPractice, OnEndPractice},
            { BeamModeFactory.kNetwork, OnEndNetworkMode},
            { BeamModeFactory.kNetPlay, OnEndNetPlay},
        };

        modeResumeActions = new Dictionary<int, Action<BeamGameMode, object>>()
        {
            { BeamModeFactory.kNetwork, OnResumeNetworkMode},
        };

        modePauseActions = new Dictionary<int, Action<BeamGameMode, object>>()
        {
            { BeamModeFactory.kNetwork, OnPauseNetworkMode},
        };

    }

    public void OnStartMode(BeamGameMode mode, object param) => modeStartActions[mode.ModeId()](mode, param);
    public void OnEndMode(BeamGameMode mode, object param) => modeEndActions[mode.ModeId()](mode, param);
    public void OnResumeMode(BeamGameMode mode, object param) => modeResumeActions[mode.ModeId()](mode, param);
    public void OnPauseMode(BeamGameMode mode, object param) => modePauseActions[mode.ModeId()](mode, param);

    protected void OnStartSplash(BeamGameMode mode, object param)
    {
        SetupSplashCamera();
        connectBtn.SetActive(true);
        ((ModeSplash)mode).FeTargetCameraEvt += OnTargetCamera;
    }
    protected void OnEndSplash(BeamGameMode mode, object param)
    {
        ((ModeSplash)mode).FeTargetCameraEvt -= OnTargetCamera;
    }

    protected void OnStartPractice(BeamGameMode mode, object param)
    {
        SetupPlayCameras();
    }
    protected void OnEndPractice(BeamGameMode mode, object param) {}

    protected void OnStartNetworkMode(BeamGameMode mode, object param)
    {
        networkStage.ShowProceedButton(false);
        SetupNetworkCamera();

        // Note that this is the FRONTEND listening.
        logger.Info($"OnStartNetworkMode(): Listening for network events");
        beamAppl.GameAnnounceEvt += OnGameAnnounceEvt;
        beamAppl.PeerJoinedEvt += OnPeerJoinedNetEvt;
        beamAppl.PeerLeftEvt += OnPeerLeftNetEvt;
    }

    protected void OnPauseNetworkMode(BeamGameMode mode, object param) {}

    protected void OnResumeNetworkMode(BeamGameMode mode, object param)
    {
        SetupNetworkCamera();
    }

    protected void OnEndNetworkMode(BeamGameMode mode, object param)
    {
        logger.Info($"OnEndNetworkMode(): No longer listening for network events");
        beamAppl.GameAnnounceEvt -= OnGameAnnounceEvt;
        beamAppl.PeerJoinedEvt -= OnPeerJoinedNetEvt;
        beamAppl.PeerLeftEvt -= OnPeerLeftNetEvt;
    }

    protected void SetupNetworkCamera()
    {
        mainObj.uiController.ClearToasts();
        mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);
        mainObj.uiController.switchToNamedStage("NetworkStage");
    }

    protected void OnStartNetPlay(BeamGameMode mode, object param)
    {
        logger.Info($"OnStartNetPlay()");
        SetupPlayCameras();
    }
    protected void OnEndNetPlay(BeamGameMode mode, object param) {}

    protected void SetupSplashCamera()
    {
        mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);
        mainObj.uiController.switchToNamedStage("SplashStage");
    }

    protected void SetupPlayCameras()
    {
        mainObj.uiController.ClearToasts();
        mainObj.gameCamera.transform.position = new Vector3(100, 100, 100);
        mainObj.uiController.switchToNamedStage("PlayStage");
    }

    protected void OnTargetCamera(object sender, StringEventArgs args)
    {
        logger.Info($"OnTargetCamera(): Setting camera to {SID(args.str)}");
        GameObject tBikeObj = GetBikeObj(args.str);
        if (tBikeObj != null)
        {
            mainObj.gameCamera.MoveCameraToTarget(tBikeObj, 5f, 2f, .5f,  0); // Sets "close enough" value to zero - so it never gets there

            mainObj.gameCamera.StartBikeMode(tBikeObj);

            int choice = UnityEngine.Random.Range(0, 4); // No orbit view until I fix it (needs to zoom to the bike before orbiting)
            switch (choice)
            {
                case 0:
                    mainObj.gameCamera.StartBikeMode(tBikeObj);
                    mainObj.uiController.ShowToast($"Follow View", Toast.ToastColor.kGreen);
                    break;
                case 1:
                    mainObj.gameCamera.StartOverheadMode(tBikeObj);
                    mainObj.uiController.ShowToast($"Overhead View", Toast.ToastColor.kGreen);
                    break;
                case 2:
                    mainObj.gameCamera.StartEnemyView(tBikeObj);
                    mainObj.uiController.ShowToast($"Target View", Toast.ToastColor.kGreen);
                    break;
                case 3:
                    mainObj.gameCamera.StartOrbitView(tBikeObj);
                    mainObj.uiController.ShowToast($"Orbit View", Toast.ToastColor.kGreen);
                    break;
            }
        }
    }

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


#if !SINGLE_THREADED

    public async Task<GameSelectedEventArgs> SelectGameAsync(IDictionary<string, BeamGameAnnounceData> existingGames)
    {
        TaskCompletionSource<GameSelectedEventArgs> tcs = new TaskCompletionSource<GameSelectedEventArgs>();

        logger.Info($"SelectGameAsync(): Current stage: {mainObj.uiController.CurrentStage()?.name},  displaying selection UI");
        GameObject panelGo = mainObj.uiController.CurrentStage().transform.Find("SelGamePanel").gameObject;
        SelGamePanel panel = panelGo.GetComponent<SelGamePanel>();
        panel.LoadAndShowAsync(existingGames, tcs);

        logger.Info($"SelectGameAsync(): awaiting task");
        return await tcs.Task;
    }

    public void OnGameSelectedAsync(GameSelectedEventArgs selection, TaskCompletionSource<GameSelectedEventArgs> tcs )
    {
        logger.Info($"OnGameSelected(): Setting result: {selection.gameInfo?.GameName} / {selection.result}");
        tcs.TrySetResult(selection);
    }

#endif
    public void SelectGame(IDictionary<string, BeamGameAnnounceData> existingGames)
    {
        logger.Info($"SelectGame(): displaying UI");
        GameObject panelGo = mainObj.uiController.CurrentStage().transform.Find("SelGamePanel").gameObject;
        SelGamePanel panel = panelGo.GetComponent<SelGamePanel>();
        panel.LoadAndShow(existingGames);
    }

    public void OnGameSelected(GameSelectedEventArgs selection)
    {
        logger.Info($"OnGameSelected(): Setting result: {selection.gameInfo?.GameName} / {selection.result}");
        beamAppl.OnGameSelected( selection.gameInfo, selection.result );
    }


    //
    // Event handlers
    //

    // Network information events

    public void OnPeerJoinedNetEvt(object sender, PeerJoinedEventArgs args)
    {
        BeamNetworkPeer p = args.peer;
        logger.Info($"OnPeerJoinedEvt() name: {p.Name}, Id: {SID(p.PeerId)}");

        UpdateNetworkInfo();

    }

    public void OnPeerLeftNetEvt(object sender, PeerLeftEventArgs args)
    {
        logger.Info($"OnPeerLeftEvt(): {SID(args.p2pId)}");

        if (appCore != null)
        {
                // ToDo: Need an OnPlayerLeft handler. This one's about the peer
                // Or... should have onPeerLeft toast?
                BeamPlayer pl = appCore?.CoreState.GetPlayer(args.p2pId);
                mainObj.uiController.ShowToast($"Player {(pl!=null?pl.Name:"<unk>")} Left Game", Toast.ToastColor.kRed,5);
        }

        UpdateNetworkInfo();
    }

    public void OnGameAnnounceEvt(object sender, GameAnnounceEventArgs args)
    {
        UpdateNetworkInfo();
    }

    // Players

    public void OnPlayerJoinedEvt(object sender, PlayerJoinedEventArgs args)
    {
        // Player joined means a group has been joined AND is synced (ready to go)
        if ( args.player.PeerId == appCore.LocalPeerId )
        {
            if (mainObj.beamApp.modeMgr.CurrentModeId() == BeamModeFactory.kNetPlay)
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
        logger.Info($"OnNewBikeEvt(). Id: {SID(ib.bikeId)}, Owner: {SID(ib.peerId)} LocalPlayer: {(BikeIsLocalPlayer(ib))}");
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
