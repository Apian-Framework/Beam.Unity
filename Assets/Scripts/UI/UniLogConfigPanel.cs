using System.Collections;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;
using TMPro;
using UniLog;


public class UniLogConfigPanel : MovableUICanvasItem
{
    public static Dictionary<UniLogger.Level, int> IndexForLevel = new Dictionary<UniLogger.Level, int>() {
        {UniLogger.Level.Debug, 5},
        {UniLogger.Level.Verbose, 4},
        {UniLogger.Level.Info, 3},
        {UniLogger.Level.Warn, 2},
        {UniLogger.Level.Error,1},
        {UniLogger.Level.Off, 0},
    };

    public static List<UniLogger.Level> LevelForIndex = new List<UniLogger.Level>() {
        UniLogger.Level.Off,
        UniLogger.Level.Error,
        UniLogger.Level.Warn,
        UniLogger.Level.Info,
        UniLogger.Level.Verbose,
        UniLogger.Level.Debug,
    };

    public GameObject LogLevelPrefab;

    public GameObject DefaultLevelDrop;

    public GameObject ScrollViewContent;

    protected IDictionary<string, GameObject> LoggerLevels;

    protected bool _isDirty; // needs sort

    protected UniLogger Logger;

    protected override void Awake()
    {
        Logger = UniLogger.GetLogger("UniLogConfig");
	    Logger.Verbose($"UniLogConfigPanel Awake");
        LoggerLevels = new Dictionary<string, GameObject>();
    }

	protected override void Start ()
	{
	    Logger.Verbose($"UniLogConfigPanel Start");
        base.Start();
	}

    protected override void Update()
    {
        base.Update();
        if (_isDirty)
            SortLoggers();
    }

    public void LoadAndShow()
    {
        _DoLoadAndShow();
    }

    protected void _DoLoadAndShow()
    {
        TMP_Dropdown drop = DefaultLevelDrop.GetComponent<TMP_Dropdown>();
        drop.value = IndexForLevel[UniLogger.DefaultLevel];
        drop.RefreshShownValue();
        Logger.Info($"UniLogConfigPanel LoadAndShow()");
        foreach (UniLogger logger in  UniLogger.AllLoggers) AddLogger(logger);
        moveOnScreen();
    }

    protected void AddLogger(UniLogger logger)
    {
        GameObject newLine = GameObject.Instantiate(LogLevelPrefab, transform);
        UniLogLevel lvl = newLine.GetComponent<UniLogLevel>();
        lvl.Setup(this, logger);
        newLine.transform.SetParent(ScrollViewContent.transform); // set it as a child of ScrollViewContent
        LoggerLevels[logger.LoggerName] = newLine;
        _isDirty = true;
    }

    protected void SortLoggers()
    {
        Vector3 pos = new Vector3(0,0,0);
        IList<string> sortedNames = LoggerLevels.Keys.OrderBy(s => s).ToList();

        float height =sortedNames.Count * 40;
  	    RectTransform rt = ScrollViewContent.GetComponent<RectTransform>();
		Vector2 sd = rt.sizeDelta;
		sd.y = height;
		rt.sizeDelta = sd;

        foreach (string name in sortedNames)
        {
            LoggerLevels[name].transform.localPosition = pos;
            pos.y -= 40;
        }
        _isDirty = false;
    }

    public void DoSetDefaultLevel(int index)
    {
        UniLogger.DefaultLevel = LevelForIndex[index];
        Logger.Info($"UniLogConfigPanel DoSetDefaultLevel() - setting to {UniLogger.DefaultLevel}");
		BeamMain.GetInstance().PersistSettings();
    }

    public void DoSetAllToDefault()
    {
        foreach (GameObject go in LoggerLevels.Values)
        {
            UniLogLevel logLvl = go.GetComponent<UniLogLevel>();
            logLvl.SetLevel(UniLogger.DefaultLevel); // note that this ends up calling PersistSettings a bunch of times
        }
    }

    public void DoClipAllToDefault()
    {
        // "More verbose" is a lower level  (higher priority)
        foreach (GameObject go in LoggerLevels.Values)
        {
            UniLogLevel logLvl = go.GetComponent<UniLogLevel>();
            if (logLvl.ControlledLogger.LogLevel < UniLogger.DefaultLevel)
            {
                logLvl.SetLevel(UniLogger.DefaultLevel); // note that this ends up calling PersistSettings a bunch of times
            }
        }
    }

    public void DoDone()
    {
        Logger.Info($"UniLogConfigPanel DoDone()");
        BeamMain.GetInstance().PersistSettings();
        moveOffScreen();
    }



}
