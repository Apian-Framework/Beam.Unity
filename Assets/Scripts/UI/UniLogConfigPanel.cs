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

    protected IDictionary<string, GameObject> levels;

    protected bool _isDirty; // needs sort

	protected override void Start ()
	{
        base.Start();
        levels = new Dictionary<string, GameObject>();
        TMP_Dropdown drop = DefaultLevelDrop.GetComponent<TMP_Dropdown>();
        drop.value = IndexForLevel[UniLogger.DefaultLevel];
        drop.RefreshShownValue();
	}

    protected override void Update ()
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
        foreach (UniLogger logger in  UniLogger.AllLoggers) AddLogger(logger);
        moveOnScreen();
    }

    protected void AddLogger(UniLogger logger)
    {
        GameObject newLine = GameObject.Instantiate(LogLevelPrefab, transform);
        UniLogLevel lvl = newLine.GetComponent<UniLogLevel>();
        lvl.Setup(this, logger);
        newLine.transform.SetParent(ScrollViewContent.transform); // set it as a child of ScrollViewContent
        levels[logger.LoggerName] = newLine;
        _isDirty = true;
    }

    protected void SortLoggers()
    {
        Vector3 pos = new Vector3(0,0,0);
        IList<string> sortedNames = levels.Keys.OrderBy(s => s).ToList();

        float height =sortedNames.Count * 40;
  	    RectTransform rt = ScrollViewContent.GetComponent<RectTransform>();
		Vector2 sd = rt.sizeDelta;
		sd.y = height;
		rt.sizeDelta = sd;


        foreach (string name in sortedNames)
        {
            levels[name].transform.localPosition = pos;
            pos.y -= 40;
        }
        _isDirty = false;
    }

    public void DoSetDefaultLevel(int index)
    {
        UniLogger.DefaultLevel = LevelForIndex[index];
    }

    public void DoDone()
    {

        moveOffScreen();
    }



}
