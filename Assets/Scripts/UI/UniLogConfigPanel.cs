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

    public int DefLogLevel;

    protected IDictionary<string, UniLogLevel> levels;

	protected override void Start ()
	{
        base.Start();
        TMP_Dropdown drop = DefaultLevelDrop.GetComponent<TMP_Dropdown>();
        DefLogLevel = drop.value;
        drop.value = IndexForLevel[UniLogger.DefaultLevel];
        drop.RefreshShownValue();
	}

    public void LoadAndShow()
    {
        _DoLoadAndShow();
    }

    protected void _DoLoadAndShow()
    {
        moveOnScreen();
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
