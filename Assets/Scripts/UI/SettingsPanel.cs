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
    public GameObject netNameField;


    public void LoadAndShow()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings settings = mainObj.frontend.GetUserSettings();

        screenNameField.GetComponent<TMP_InputField>().text = settings.screenName;
        p2pConnectionField.GetComponent<TMP_InputField>().text = settings.p2pConnectionString;
        ethNodeField.GetComponent<TMP_InputField>().text = settings.ethNodeUrl;
        ethAcctField.GetComponent<TMP_InputField>().text = settings.ethAcct;
        netNameField.GetComponent<TMP_InputField>().text = settings.apianNetworkName;

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
        settings.apianNetworkName = netNameField.GetComponent<TMP_InputField>().text;

        UserSettingsMgr.Save(settings);

        moveOffScreen();
    }

}
