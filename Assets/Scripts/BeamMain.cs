using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class BeamMain : MonoBehaviour
{

	public GameCamera gameCamera;
	public UICamera uiCamera;	
	public InputDispatch inputDispatch;    
	public EthereumProxy eth = null;
    public GameObject boomPrefab;      

    // Non-monobehaviors
    public BeamGameInstance backend;
    public FrontendProxy feProxy;    

    void Awake() {
        DontDestroyOnLoad(transform.gameObject); // this obj survives scene change (TODO: Needed?)
    }	
    // Start is called before the first frame update
    void Start()
    {
		Application.targetFrameRate = 30;

		// Semi-presistent Main-owned objects
		uiCamera = (UICamera)utils.findObjectComponent("UICamera", "UICamera");		
		gameCamera = (GameCamera)utils.findObjectComponent("GameCamera", "GameCamera");		
		
        eth = new EthereumProxy();
		eth.ConnectAsync(EthereumProxy.InfuraRinkebyUrl); // consumers should check eth.web3 before using        
		
        feProxy = new FrontendProxy();
        backend = new BeamGameInstance(feProxy);
        backend.Start();
        
    }

    // Update is called once per frame
    void Update()
    {
        backend.Loop(GameTime.DeltaTime());
    }
      
}
