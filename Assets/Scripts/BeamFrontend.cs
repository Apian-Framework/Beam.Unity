using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeamBackend;
using UniLog;

public class BeamFrontend : MonoBehaviour, IBeamFrontend
{

	public FeGround feGround;
    public GameObject connectBtn;
    public const string kSettingsFileBaseName = "unitybeamsettings";
    protected Dictionary<string, GameObject> feBikes;
    protected BeamMain mainObj;
    public IBeamAppCore appCore;

    protected BeamUserSettings userSettings;
    protected BeamFeModeHelper _feModeHelper;
    public UniLogger logger;

    // Start is called before the first frame update
    void Start()
    {
        userSettings = UserSettingsMgr.Load(kSettingsFileBaseName);
        //userSettings = BeamUserSettings.CreateDefault();
        userSettings.localPlayerCtrlType = BikeFactory.LocalPlayerCtrl; // Kinda hackly

        mainObj = BeamMain.GetInstance();
        _feModeHelper = new BeamFeModeHelper(mainObj);
        feBikes = new Dictionary<string, GameObject>();
        logger = UniLogger.GetLogger("Frontend");
    }


    public void SetAppCore(IBeamAppCore core)
    {
        appCore = core;
        if (core == null)
            return;

        OnNewCoreState(null, appCore.CoreData);

        appCore.NewCoreStateEvt += OnNewCoreState;
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

    //
    // IBeamFrontend API
    //

    public BeamUserSettings GetUserSettings() => userSettings;

    public void OnStartMode(int modeId, object param) =>  _feModeHelper.OnStartMode(modeId, param);
    public void OnEndMode(int modeId, object param) => _feModeHelper.OnEndMode(modeId, param);

    public void OnNewCoreState(object sender, BeamCoreState newCoreState)
    {
        newCoreState.PlaceFreedEvt += OnPlaceFreedEvt;
        newCoreState.PlacesClearedEvt += OnPlacesClearedEvt;
        newCoreState.SetupPlaceMarkerEvt += OnSetupPlaceMarkerEvt;
    }


    // Players

    public void OnPeerJoinedGameEvt(object sender, PeerJoinedGameArgs args)
    {
    //      BeamPeer p = args.peer;
    //      logger.Info($"New Peer: {p.Name}, Id: {p.PeerId}");
    }

    public void OnPeerLeftGameEvt(object sender, PeerLeftGameArgs args)
    {
         logger.Info("Peer Left: {args.p2pId}");
    }

    public void OnPlayersClearedEvt(object sender, EventArgs e)
    {
        logger.Info("OnPlayersClearedEvt() currently does nothing");
    }

    // Bikes

    public void OnNewBikeEvt(object sender, IBike ib)
    {
        logger.Info($"OnNewBikeEvt(). Id: {ib.bikeId}, LocalPlayer: {ib.ctrlType == BikeFactory.LocalPlayerCtrl}");
        GameObject bikeGo = FrontendBikeFactory.CreateBike(ib, feGround);
        feBikes[ib.bikeId] = bikeGo;
        if (ib.ctrlType == BikeFactory.LocalPlayerCtrl)
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

        IBike ib = appCore.CoreData.GetBaseBike(rData.bikeId);
        feBikes.Remove(rData.bikeId);
        mainObj.uiController.CurrentStage().transform.Find("Scoreboard")?.SendMessage("RemoveBike", go);
        if (ib.ctrlType == BikeFactory.LocalPlayerCtrl)
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
    public void OnPlaceClaimedEvt(object sender, BeamPlace p) {}

    // Ground
    public void OnSetupPlaceMarkerEvt(object sender, BeamPlace p)
    {
        feGround.SetupMarkerForPlace(p);
    }
    //public void OnFreePlace(BeamPlace p, int modeId)
    public void OnPlaceFreedEvt(object sender, BeamPlace p)
    {
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
