using System;
using UnityEngine;
using BeamBackend;

public class BeamMain : MonoBehaviour
{

	public GameCamera gameCamera;
	public UICamera uiCamera;	
	public InputDispatch inputDispatch;    
	public EthereumProxy eth = null;
    public GameObject boomPrefab;      
    public FeGround feGround;

    // Non-monobehaviors
    public BeamGameInstance backend;
    public FrontendProxy feProxy;    

    // Singletone management(*yeah, kinda lame)
    private static BeamMain instance = null;
    public static BeamMain GetInstance()
    {
        if (instance == null)
        {
            instance = (BeamMain)GameObject.FindObjectOfType(typeof(BeamMain));
            if (!instance)
                Debug.LogError("There needs to be one active BeamMain script on a GameObject in your scene.");
        }
 
        return instance;
    }    


    void Awake() {
        DontDestroyOnLoad(transform.gameObject); // this obj survives scene change (TODO: Needed?)
    }	
    // Start is called before the first frame update
    void Start()
    {
        UnityEngine.Debug.Log("BeamMain.Start() starting...");        
		Application.targetFrameRate = 30;

		// Semi-presistent Main-owned objects 
        // TODO: Should be in Awake()?
		uiCamera = (UICamera)utils.findObjectComponent("UICamera", "UICamera");		
		gameCamera = (GameCamera)utils.findObjectComponent("GameCamera", "GameCamera");		
		
        eth = new EthereumProxy();
		eth.ConnectAsync(EthereumProxy.InfuraRinkebyUrl); // consumers should check eth.web3 before using        
		
        feProxy = new FrontendProxy();
        backend = new BeamGameInstance(feProxy);
        backend.Start();
        UnityEngine.Debug.Log("BeamMain.Start() done");
        
    }

    // Update is called once per frame
    void Update()
    {      
        backend.Loop(GameTime.DeltaTime());
    }
      
    public void HandleTap(bool isDown) // true is down
    {
        throw(new Exception("Not Implmented"));        
    }

}
