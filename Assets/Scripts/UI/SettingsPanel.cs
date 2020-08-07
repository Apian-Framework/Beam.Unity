using System;
using System.Collections.Generic;
using BeamGameCode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SettingsPanel : MovableUICanvasItem
{
    public GameObject screenNameField;
    public GameObject p2pConnectionField;
    public GameObject ethNodeField;
    public GameObject ethAcctField;
     public GameObject gameIdField;

    public void LoadAndShow()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings settings = mainObj.frontend.GetUserSettings();

        screenNameField.GetComponent<TMP_InputField>().text = settings.screenName;
        p2pConnectionField.GetComponent<TMP_InputField>().text = settings.p2pConnectionString;
        ethNodeField.GetComponent<TMP_InputField>().text = settings.ethNodeUrl;
        ethAcctField.GetComponent<TMP_InputField>().text = settings.ethAcct;
        gameIdField.GetComponent<TMP_InputField>().text =
            settings.tempSettings.ContainsKey("gameId") ? settings.tempSettings["gameId"] : "";

        UserSettingsMgr.Save(settings);

        moveOnScreen();
    }

    public void SaveAndHide()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings settings = mainObj.frontend.GetUserSettings();

        settings.screenName = screenNameField.GetComponent<TMP_InputField>().text;
        settings.p2pConnectionString = p2pConnectionField.GetComponent<TMP_InputField>().text;
        settings.ethNodeUrl = ethNodeField.GetComponent<TMP_InputField>().text;
        settings.ethAcct = ethAcctField.GetComponent<TMP_InputField>().text;
        string gameId = gameIdField.GetComponent<TMP_InputField>().text;
        if (gameId != "")
            settings.tempSettings["gameId"] = gameId;

        UserSettingsMgr.Save(settings);

        moveOffScreen();
    }



}
