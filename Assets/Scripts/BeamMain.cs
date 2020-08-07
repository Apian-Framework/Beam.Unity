using System;
using System.Diagnostics;
using UnityEngine;
using BeamGameCode;
using Unity.Profiling;
using UniLog;

public class BeamMain : MonoBehaviour
{
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
        DontDestroyOnLoad(transform.gameObject); // this obj survives scene change (TODO: Needed?)
    }
    // Start is called before the first frame update
    void Start()
    {
		Application.targetFrameRate = 60;

		// Semi-presistent Main-owned objects
        // TODO: Should be in Awake()?
        frontend = (BeamFrontend)utils.findObjectComponent("BeamFrontend", "BeamFrontend");
		uiController = (GameUiController)utils.findObjectComponent("GameUiController", "GameUiController");
		gameCamera = (GameCamera)utils.findObjectComponent("GameCamera", "GameCamera");
        gameNet = new BeamGameNet();
        beamApp = new BeamApplication(gameNet, frontend);
        beamApp.Start(BeamModeFactory.kSplash);

        inputDispatch = new InputDispatch(this);

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

}
