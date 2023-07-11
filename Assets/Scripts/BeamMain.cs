using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;
using BeamGameCode;
using Unity.Profiling;
using UniLog;

public class PlatformSettings
{
    protected const string kMasterVolume = "mastervolume";
    protected const string kApianGameBase = "apiangamebase";
    protected const string kEnableLogLvlEdit = "enableLogLvlEdit";

    public const float defMasterVolume = .5f;
    public const bool defEnableLogLvlEdit = false;

    public float masterVolume = defMasterVolume;
    public string apianGameBase = "beam"; // default
    public bool enableLogLvlEdit = defEnableLogLvlEdit;

    public void Load( Dictionary<string,string> settingsDict)
    {
        if ( settingsDict.ContainsKey(kMasterVolume) )
            if (! float.TryParse(settingsDict[kMasterVolume], out masterVolume) )
                masterVolume = defMasterVolume;

        if (settingsDict.ContainsKey(kEnableLogLvlEdit))
            if (! bool.TryParse(settingsDict[kEnableLogLvlEdit], out enableLogLvlEdit) )
                enableLogLvlEdit= defEnableLogLvlEdit;

        apianGameBase = settingsDict.ContainsKey(kApianGameBase) ? settingsDict[kApianGameBase] : apianGameBase;


        // On load create a randomish game/group spec and store it in tempsettings
        //BeamMain.GetInstance().frontend.GetUserSettings().tempSettings["gameSpec"] = CreateNewGameSpec(apianGameBase);
    }

    public Dictionary<string, string> ToDict()
    {
        return new Dictionary<string,string>()
        {
            {kMasterVolume, $"{masterVolume}" },
            {kApianGameBase, $"{apianGameBase}" },
            {kEnableLogLvlEdit, $"{enableLogLvlEdit}"}
        };
    }

    protected static List<string> shortAdjs = new List<string>()
        { "angry", "bad", "big", "busy", "cheap", "clear", "close", "cold", "cool", "dark",
        "dry", "early", "easy", "empty", "fake", "false", "far", "fast", "fine", "first", "flat", "free", "full", "good",
        "great", "happy", "hard", "heavy", "high", "hot", "huge", "large", "last", "late", "light", "long", "low", "mean",
        "near", "new", "next", "nice", "noisy", "old", "open", "poor", "quick", "ready", "real", "rich", "right", "sad", "safe",
        "same", "short", "slow", "small", "soft", "sure", "tall", "tiny", "tired", "true", "warm", "weak", "wet", "wrong",
        "young" };
    protected static List<string> shortNouns = new List<string>()
        {"word", "pen", "class", "sound", "water", "side", "place", "year",
        "day", "week", "month", "name", "line", "air", "land", "home", "hand", "house", "world", "page", "plant",
        "food", "sun", "state", "eye", "city", "tree", "farm", "story", "sea", "night", "day", "life",
         "paper", "music", "river", "car", "foot", "feet", "book", "room", "idea", "fish", "horse",
        "witch", "color", "face", "wood", "list", "bird", "body", "dog", "song", "door", "wind", "ship", "area", "rock",
        "order", "fire", "piece", "top", "king", "space"};

    // public static string CreateNewGameSpec(string apianGameBase)
    // {
    //     string adj = shortAdjs[UnityEngine.Random.Range(0, shortAdjs.Count)];
    //     string noun = shortNouns[UnityEngine.Random.Range(0, shortNouns.Count)];
    //     string gameNumStr = UnityEngine.Random.Range(0, 1000).ToString("D4");
    //     return $"{apianGameBase}{gameNumStr}/{adj}{noun}+";  // by default use "create if not exists"
    // }

}

public class BeamMain : MonoBehaviour
{
    public PlatformSettings platformSettings;
    public BeamFrontend frontend;
	public GameCamera gameCamera;
	public GameUiController uiController;
	public InputDispatch inputDispatch;
    public GameObject boomPrefab;
	// public EthereumProxy eth = null;

    static ProfilerMarker gameNetPerfMarker = new ProfilerMarker("Beam.GameNet");
    static ProfilerMarker backendPerfMarker = new ProfilerMarker("Beam.Backend");

    // Non-monobehaviors
    public UnityBeamGameNet gameNet;
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
		Application.targetFrameRate = -1; //60; // default is 60 - but in WebGL it means not to use a timer
        DontDestroyOnLoad(transform.gameObject); // this obj survives scene change (TODO: Needed?)

        platformSettings = new PlatformSettings();

        frontend = (BeamFrontend)utils.findObjectComponent("BeamFrontend", "BeamFrontend"); // loads settings
		uiController = (GameUiController)utils.findObjectComponent("GameUiController", "GameUiController");
		gameCamera = (GameCamera)utils.findObjectComponent("GameCamera", "GameCamera");

    }
    // Start is called before the first frame update
    void Start()
    {
        gameNet = new UnityBeamGameNet();
        beamApp = new BeamApplication(gameNet, frontend);
        inputDispatch = new InputDispatch(this);
    }

    // Update is called once per frame
    void Update()
    {
        //gameNetPerfMarker.Begin();
        gameNet.Update();
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
    public void ApplyPlatformUserSettings()
    {
        // Called when the frontend starts up
        platformSettings.Load(frontend.GetUserSettings().platformSettings);
        SetMasterVolume(platformSettings.masterVolume); // first time
        frontend.EnableLogLevelBtn(platformSettings.enableLogLvlEdit);
    }

    public void PersistSettings()
    {
        // TODO: is this dopey? Should BeamMain should be the keeper of the user settings?
        frontend.GetUserSettings().platformSettings = platformSettings.ToDict();
        UserSettingsMgr.Save(frontend.GetUserSettings());
    }

    public void SetMasterVolume(float v)
    {
        AudioListener.volume = v;
        if (platformSettings.masterVolume != v)
        {
            platformSettings.masterVolume = v;
            PersistSettings();
        }
    }

}
