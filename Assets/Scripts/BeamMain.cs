using System;
using System.Diagnostics;
using UnityEngine;
using BeamBackend;
using UniLog;

public class BeamMain : MonoBehaviour
{
    public BeamFrontend frontend;
	public GameCamera gameCamera;
	public UICamera uiCamera;	
	public InputDispatch inputDispatch;    
    public GameObject boomPrefab;          
	public EthereumProxy eth = null;


    // Non-monobehaviors
    public BeamGameNet gameNet;    
    public BeamGameInstance backend;

    // Singletone management(*yeah, kinda lame)
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
		Application.targetFrameRate = 30;

		// Semi-presistent Main-owned objects 
        // TODO: Should be in Awake()?
        frontend = (BeamFrontend)utils.findObjectComponent("BeamFrontend", "BeamFrontend");	       
		uiCamera = (UICamera)utils.findObjectComponent("UICamera", "UICamera");		
		gameCamera = (GameCamera)utils.findObjectComponent("GameCamera", "GameCamera");		
		
        inputDispatch = new InputDispatch(this);

        // TODO: get rid of this Eth stuff (goes in GameNet)
        eth = new EthereumProxy();
		eth.ConnectAsync(EthereumProxy.InfuraRinkebyUrl); // consumers should check eth.web3 before using        

        gameNet = new BeamGameNet();  

        backend = new BeamGameInstance((IBeamFrontend)frontend, gameNet);
        gameNet.Init(backend);
        backend.Start(BeamModeFactory.kSplash);
        
    }

    // Update is called once per frame
    void Update()
    {      
        gameNet.Loop();
        backend.Loop(GameTime.DeltaTime());
    }
      
    public void HandleTap(bool isDown) // true is down
    {
        //throw(new Exception("Not Implmented"));    
        UnityEngine.Debug.Log("** BeamMain.HandleTap() fallthru not implmented");            
    }

}
