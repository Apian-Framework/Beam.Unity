using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;
using BeamGameCode;
using Unity.Profiling;
using UniLog;

public class DriverSettings
{
    protected const string kMasterVolume = "mastervolume";

    public float masterVolume = .75f;

    public void Load( Dictionary<string,string> settingsDict)
    {
        masterVolume = settingsDict.ContainsKey(kMasterVolume) ? float.Parse(settingsDict[kMasterVolume]) : masterVolume;
    }

    public Dictionary<string, string> ToDict()
    {
        return new Dictionary<string,string>()
        {
            {kMasterVolume, $"{masterVolume}" }
        };
    }

}

public class BeamMain : MonoBehaviour
{
    public DriverSettings driverSettings;
    public BeamFrontend frontend;
	public GameCamera gameCamera;
	public GameUiController uiController;
	public InputDispatch inputDispatch;
    public GameObject boomPrefab;
	// public EthereumProxy eth = null;

    static ProfilerMarker gameNetPerfMarker = new ProfilerMarker("Beam.GameNet");
    static ProfilerMarker backendPerfMarker = new ProfilerMarker("Beam.Backend");

    // Non-monobehaviors
    public BeamGameNet gameNet;
    public BeamApplication beamApp;

    // Singleton management(*yeah, kinda lame)
    private static BeamMain instance = null;
    public static BeamMain GetInstance()
    {
        if (instance == null)
        {
            instance = (BeamMain)GameObject.FindObjectOfType(typeof(BeamMain));
            if (!instance)
                UnityEngine.Debug.LogError("There needs to be one active BeamMain script on a GameObject in your scene.");
        }

        return instance;
    }

    void Awake() {
		Application.targetFrameRate = 60;
        DontDestroyOnLoad(transform.gameObject); // this obj survives scene change (TODO: Needed?)

        driverSettings = new DriverSettings();
        frontend = (BeamFrontend)utils.findObjectComponent("BeamFrontend", "BeamFrontend");
		uiController = (GameUiController)utils.findObjectComponent("GameUiController", "GameUiController");
		gameCamera = (GameCamera)utils.findObjectComponent("GameCamera", "GameCamera");
        gameNet = new BeamGameNet();
        beamApp = new BeamApplication(gameNet, frontend);

        inputDispatch = new InputDispatch(this);

    }
    // Start is called before the first frame update
    void Start()
    {
        beamApp.Start(BeamModeFactory.kSplash);

        // TODO: get rid of this Eth stuff (goes in GameNet)
        //eth = new EthereumProxy();
		//eth.ConnectAsync(EthereumProxy.InfuraRinkebyUrl); // consumers should check eth.web3 before using
    }

    // Update is called once per frame
    void Update()
    {
        //gameNetPerfMarker.Begin();
        gameNet.Loop();
        //gameNetPerfMarker.End();

        //backendPerfMarker.Begin();
        beamApp.Loop(GameTime.DeltaTime());
        //backendPerfMarker.End();
    }

    public void HandleTap(bool isDown) // true is down
    {
        //throw(new Exception("Not Implmented"));
        UnityEngine.Debug.Log("** BeamMain.HandleTap() fallthru not implmented");
    }

    // Settings-related stuff
    public void ApplyUserSettings()
    {
        // Called when the frontend starts up
        driverSettings.Load(frontend.GetUserSettings().driverSettings);
        SetMasterVolume(driverSettings.masterVolume); // first time

    }

    public void PersistSettings()
    {
        // TODO: is this dopey? Should BeamMain should be the keeper of the user settings?
        frontend.GetUserSettings().driverSettings = driverSettings.ToDict();
        UserSettingsMgr.Save(frontend.GetUserSettings());
    }

    public void SetMasterVolume(float v)
    {
        AudioListener.volume = v;
        if (driverSettings.masterVolume != v)
        {
            driverSettings.masterVolume = v;
            PersistSettings();
        }
    }

}
