using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using UniLog;

public class UniLogLevel : MonoBehaviour
 {

	protected UniLogConfigPanel configPanel;
	public UniLogger ControlledLogger; // the one this entry is about
	protected UniLogger Logger; // this is for this code to write log messages


	public void Awake()
	{
        Logger = UniLogger.GetLogger("UniLogConfig");
		Logger.Verbose($"UniLogLevel Awake");
	}
	public  void Start ()
	{
	Logger.Verbose($"UniLogLevel Awake");
	}

	// Update is called once per frame
	public void Update ()
	{

	}

	public void Setup(UniLogConfigPanel _panel, UniLogger logger)
	{
		Logger.Verbose($"UniLogLevel setup( {logger.LoggerName}");
		ControlledLogger = logger;
		configPanel = _panel;
		SetText(logger.LoggerName);
		SetLevel(logger.LogLevel);
	}

	public void SetText(string msg)
	{
		Logger.Verbose($"UniLogLevel.SetText({msg})");
  	    TMP_Text tmpt = gameObject.transform.Find("Text").GetComponent<TMP_Text>();
		tmpt.text = msg;
		tmpt.ForceMeshUpdate();
	}

	public void OnLevelChanged(int index)
	{
	    Logger.Info($"UniLogLevel.OnLevelChanged(): {ControlledLogger.LoggerName} = {index}");
		ControlledLogger.LogLevel  = UniLogConfigPanel.LevelForIndex[index];
		BeamMain.GetInstance().PersistSettings();
	}
	public void SetLevel(UniLogger.Level lvl)
	{
		// badly named - just sets the dropdown
		// Note that this also calls "OnValueChanged" for thr dropdown, which calls OnLevelChanged()
		// above, which actually sets the UniLog level and persists all user setting
		Logger.Verbose($"UniLogLevel.SetLevel({lvl})");
		TMP_Dropdown drp = gameObject.transform.Find("Lvl").GetComponent<TMP_Dropdown>();
		drp.value = UniLogConfigPanel.IndexForLevel[lvl];
	}


}
