using System;
using System.Collections.Generic;
using System.Linq;
using BeamGameCode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class SettingsPanel : MovableUICanvasItem
{
    public GameObject screenNameField;
    public GameObject p2pConnectionDrop;
    public GameObject blockchainDrop;
    public GameObject gameAcctDrop;
    public GameObject permAcctField;
    public GameObject netNameField;
    public GameObject logLvlEditField;

    protected void _SetupDropdown(TMP_Dropdown drop, List<string> options, string defaultOption)
    {
        drop.ClearOptions();
        drop.AddOptions(options);
        drop.value = options.FindIndex(option => option == defaultOption);
    }




    public void LoadAndShow()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings settings = mainObj.frontend.GetUserSettings();

        _SetupDropdown( p2pConnectionDrop.GetComponent<TMP_Dropdown>(),
            settings.p2pConnectionSettings.Keys.ToList(),
            settings.curP2pConnection
        );

        _SetupDropdown( blockchainDrop.GetComponent<TMP_Dropdown>(),

            settings.blockchainInfos.Keys.ToList(),
            settings.curBlockchain
        );

        _SetupDropdown( gameAcctDrop.GetComponent<TMP_Dropdown>(),

            settings.gameAcctJSON.Keys.ToList(),
            settings.gameAcctAddr
        );

        permAcctField.GetComponent<TMP_InputField>().text = settings.permAcctAddr;
        netNameField.GetComponent<TMP_InputField>().text = settings.apianNetworkName;
        screenNameField.GetComponent<TMP_InputField>().text = settings.screenName;

        logLvlEditField.GetComponent<Toggle>().isOn = bool.Parse(settings.platformSettings.TryGetValue("enableLogLvlEdit", out var x) ? x : "false");

        UserSettingsMgr.Save(settings);

        moveOnScreen();
    }

    public void SaveAndHide()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings settings = mainObj.frontend.GetUserSettings();

        settings.curP2pConnection = p2pConnectionDrop.GetComponent<TMP_Dropdown>().captionText.text;
        settings.curBlockchain = blockchainDrop.GetComponent<TMP_Dropdown>().captionText.text;
        settings.gameAcctAddr = gameAcctDrop.GetComponent<TMP_Dropdown>().captionText.text;

        settings.permAcctAddr = permAcctField.GetComponent<TMP_InputField>().text;
        settings.apianNetworkName = netNameField.GetComponent<TMP_InputField>().text;
        settings.screenName = screenNameField.GetComponent<TMP_InputField>().text;

        mainObj.platformSettings.enableLogLvlEdit = logLvlEditField.GetComponent<Toggle>().isOn; // unity-only setting

        mainObj.PersistSettings();
        mainObj.ApplyPlatformUserSettings();

        moveOffScreen();
    }

}
