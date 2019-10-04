using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using BeamBackend;


// A  "Mode" the game can be in
// Also used for "subModes"

public abstract class GameMode
{	
    protected Dictionary<int,dynamic> _cmdDispatch = new Dictionary<int, dynamic>();  	
	public OldGameMain _mainObj; 
		
	public virtual void init()
	{
		_mainObj = (OldGameMain)utils.findObjectComponent("OldGameMain", "OldGameMain");
	}
	public virtual void update() {}
	public virtual void end() {}
    public virtual void HandleTap(bool isDown) {}        

    public virtual void handleCmd(int cmd, object param) {}  	   	
};

public abstract class ModeState
{    
    public virtual void init() {}
    public virtual void update() {}
    public virtual void end() {}
    public virtual void pause() {}
    public virtual void resume() {}
    
   //  public virtual void doNavBtn(GameNavBtn.Direction dir) {}
    public virtual void HandleTap(bool isDown) {}    
};


public class OldGameMain : MonoBehaviour {
	
	public EthereumProxy eth = null;

	public BackendMain backend { get; private set; } = null;

	public Dictionary<string, GameObject> BikeList { get; private set;}	
	public GameCamera gameCamera;
	public UICamera uiCamera;	
	public InputDispatch inputDispatch;
	
	public FeGround feGround;
    public GameObject boomPrefab;  	

	// here's the currently implemented modes:
	public enum ModeID
	{
        kSplash,
		kPlay
	};
 
    // Singleton management
    private static OldGameMain instance = null;
    public static OldGameMain GetInstance()
    {
        if (instance == null)
        {
            instance = (OldGameMain)GameObject.FindObjectOfType(typeof(OldGameMain));
            if (!instance)
                Debug.LogError("There needs to be one active GameMain script on a GameObject in your scene.");
        }
 
        return instance;
    }    
    
    //
    // Lifecycle
    //
    
	
	protected GameMode _curMode = null;
	
	bool firstFrame = true;
	
	// Use this for initialization
	void Start () {
		Application.targetFrameRate = 30;

		// Shared game state - presistent
		backend = new BackendMain();
		
		backend.EventPub.BikeAtPoint += bikeAtPointHandler;

		// Semi-presistent GameMain-owned objects
		BikeList = new Dictionary<string, GameObject>();
		uiCamera = (UICamera)utils.findObjectComponent("UICamera", "UICamera");		
		gameCamera = (GameCamera)utils.findObjectComponent("GameCamera", "GameCamera");		
		eth = new EthereumProxy();

		eth.ConnectAsync(EthereumProxy.InfuraRinkebyUrl); // consumers should check eth.web3 before using
		//eth.Connect(EthereumProxy.InfuraRinkebyUrl); // consumers should check eth.web3 before using		
		firstFrame = true;
	}
	
    void Awake() {
        DontDestroyOnLoad(transform.gameObject);
    }	
	
	// Update is called once per frame
	void Update () 
	{

		backend.DoUpdate(GameTime.DeltaTime()); // update underlying game data first

		if (firstFrame)
		{	
			// do first frame stuff	          
            setGameMode(OldGameMain.ModeID.kSplash);  // gives everything a chance to Start()  
            uiCamera.setToNamedStage("SplashStage"); 			 
			firstFrame = false;
		}
		
		if (_curMode != null)
		{
			_curMode.update();	
		}		
	}
	
	// catch a "game nav" button
	// public void doNavBtn(GameNavBtn.Direction navDir)
	// {
	// 	if (_curMode != null)
	// 		_curMode.doNavBtn(navDir);
	// }
    
    public void HandleTap(bool isDown)
    {
        if (_curMode != null)
            _curMode.HandleTap(isDown);
    }    
	
	public void setGameMode(ModeID mode)
	{
		GameMode newMode = null;
		
		switch (mode)
		{
        case ModeID.kSplash:
            newMode = new GameModeSplash();
            break;

		case ModeID.kPlay:
			newMode = new GameModePlay();
			break;
               
		}
		
		if (newMode != null)	
		{
			if (_curMode != null)
				_curMode.end();		
			_curMode = null;			
			_curMode = newMode;		
			_curMode.init();				
		}	
	}

    public GameMode GetCurrentMode()
    {
        return _curMode;   
    }

	public void SendCmd(int cmd, object param)
	{
        if (_curMode != null)
            _curMode.handleCmd(cmd, param );  		
	}

	public void DestroyBikes()
	{
		foreach (GameObject bk in BikeList.Values)
		{
			GameObject.Destroy(bk);
		}
		BikeList.Clear();
	}

	public void RemoveOneBike(GameObject bikeObj)
	{
		BaseBike bb = bikeObj.GetComponent<FrontendBike>().bb;
		BikeList.Remove(bb.bikeId);
		uiCamera.CurrentStage().transform.Find("Scoreboard")?.SendMessage("RemoveBike", bikeObj); // TODO: find better way
		if (inputDispatch.localPlayerBike != null && bikeObj == inputDispatch.localPlayerBike.gameObject)
		{
			Debug.Log("Boom! Player");
			uiCamera.CurrentStage().transform.Find("RestartCtrl")?.SendMessage("moveOnScreen", null); 
		}
		feGround.beGround.RemovePlacesForBike(bb); // TODO: call to removeBike should do this
		backend.RemoveBike(bb); 
		GameObject.Instantiate(boomPrefab, bikeObj.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(bikeObj);
	}

	public void ReportScoreEvent(FrontendBike bike, ScoreEvent evt, Ground.Place place)
	{
		int scoreDelta = GameConstants.eventScores[(int)evt];
		bike.player.Score += scoreDelta;

		if (evt == ScoreEvent.kHitEnemyPlace || evt == ScoreEvent.kHitFriendPlace)
		{
			// half of the deduction goes to the owner of the place, the rest is divded 
			// among the owner's team 
			// UNLESS: the bike doing the hitting IS the owner - then the rest of the team just splits it
			if (bike.bb != place.bike) {
				scoreDelta /= 2;
				place.bike.player.Score -= scoreDelta; // adds
			}

			IEnumerable<FrontendBike> rewardedOtherBikes = 
				BikeList.Values.Select(go =>  go.transform.GetComponent<FrontendBike>()) // all bikes (yuk! fix this)
				.Where( b => b != bike && b.player.Team == place.bike.player.Team);  // Bikes other the "bike" on affected team

			if (rewardedOtherBikes.Count() > 0)
			{
				foreach (FrontendBike b in rewardedOtherBikes) 
					b.player.Score -= scoreDelta / rewardedOtherBikes.Count();
			}
		}

		if (evt == ScoreEvent.kOffMap) 
		{
			bike.player.Score = 0; // Boom!
		}
	}

	//
	// C# event handlers  
	// TODO: probably need to stop calling other, regular messaging calls "Events"
	//
	public void bikeAtPointHandler(object sender, FrontendProxy.BikeAtPointArgs args)
	{
		GameObject bike = BikeList[args.bikeId];
		//bike.transform.GetComponent<FrontendBike>().DealWithPlace(args.pos);
	}
}
