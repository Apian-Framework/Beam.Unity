using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using UniLog;

public class UniLogLevel : MonoBehaviour
 {

	protected UniLogConfigPanel configPanel;

	// Use this for initialization
	protected  void Start ()
	{

	}

	// Update is called once per frame
	protected void Update ()
	{

	}

	public void Setup(UniLogConfigPanel _panel, string _loggerName, UniLogger.Level lvl)
	{
		configPanel = _panel;
		SetText(_loggerName);
		SetLevel(lvl);

	}

	public void SetText(string msg)
	{
		 TMP_Text tmpt = gameObject.transform.Find("Text").GetComponent<TMP_Text>();
		 tmpt.text = msg;
		 tmpt.ForceMeshUpdate();
	}

	public void SetLevel(UniLogger.Level lvl)
	{
		TMP_Dropdown drp = gameObject.transform.Find("Lvl").GetComponent<TMP_Dropdown>();
		drp.value = (int)lvl;
	}


}
