using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


// A  "Mode" the game can be in
// Also used for "subModes"

public abstract class GameMode
{	
	public GameMain _mainObj; 
		
	public virtual void init()
	{
		_mainObj = (GameMain)utils.findObjectComponent("GameMain", "GameMain");
	}
	public virtual void update() {}
	public virtual void end() {}
    public virtual void HandleTap(bool isDown) {}        
	// public virtual void doNavBtn(GameNavBtn.Direction dir) {} //&&& need more generic button event
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


public class GameMain : MonoBehaviour {
	
	public EthereumProxy eth = null;
	public List<GameObject> BikeList { get; private set;}	
	public GameCamera gameCamera;
	public UICamera uiCamera;	
	public InputDispatch inputDispatch;
	public Ground ground;

	// here's the currently implemented modes:
	public enum ModeID
	{
        kSplash,
		kPlay
	};
 
    // Singleton management
    private static GameMain instance = null;
    public static GameMain GetInstance()
    {
        if (instance == null)
        {
            instance = (GameMain)GameObject.FindObjectOfType(typeof(GameMain));
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

		// Semi-presistent GameMain-owned objects
		BikeList = new List<GameObject>();
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
	
		if (firstFrame)
		{	
			// do first frame stuff	          
            setGameMode(GameMain.ModeID.kSplash);  // gives everything a chance to Start()  
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


	public void DestroyBikes()
	{
		foreach (GameObject bk in BikeList)
		{
			GameObject.Destroy(bk);
		}
		BikeList.Clear();
	}

	// public void RemoveOneBike(GameObject bikeObj)
	// {
	// 	BikeList.Remove(bikeObj);
	// 	uiCamera.CurrentStage().transform.Find("Scoreboard")?.SendMessage("RemoveBike", bikeObj); // TODO: find better way
	// 	Object.Destroy(bikeObj);
	// 	// TODO: What if it is the player? Boom?
	// }

	public void ReportScoreEvent(Bike bike, ScoreEvent evt, Ground.Place place)
	{
		int scoreDelta = GameConstants.eventScores[(int)evt];
		bike.player.Score += scoreDelta;

		if (evt == ScoreEvent.kHitEnemyPlace || evt == ScoreEvent.kHitFriendPlace)
		{
			// half of the deduction goes to the owner of the place, the rest is divded 
			// among the owner's team 
			// UNLESS: the bike doing the hitting IS the owner - then the rest of the team just splits it
			if (bike != place.bike) {
				scoreDelta /= 2;
				place.bike.player.Score -= scoreDelta; // adds
			}

			IEnumerable<Bike> rewardedOtherBikes = 
				BikeList.Select(go =>  go.transform.GetComponent<Bike>()) // all bikes (yuk! fix this)
				.Where( b => b != bike && b.player.Team == place.bike.player.Team);  // Bikes other the "bike" on affected team

			if (rewardedOtherBikes.Count() > 0)
			{
				foreach (Bike b in rewardedOtherBikes) 
					b.player.Score -= scoreDelta / rewardedOtherBikes.Count();
			}
		}		
	}

}
