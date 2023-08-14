using System;
using System.Collections.Generic;
using System.Linq;
using BeamGameCode;
using ApianCrypto;
using GameNet;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections;

public class SettingsPanel : MovableUICanvasItem
{

    public GameObject p2pConnectionDrop;
    public GameObject blockchainDrop;
    public GameObject anchorAddrField;
    public GameObject gameAcctDrop;
    public GameObject permAcctField;
    public GameObject netNameField;
    public GameObject screenNameField;
    public GameObject logLvlEditField;

    public BeamUserSettings oldSettingsForCancel;

    public const string kNoChainStr = "No Chain";

    protected void _SetupDropdown(TMP_Dropdown drop, List<string> options, string defaultOption)
    {
        drop.ClearOptions();
        drop.AddOptions(options);
        drop.value = options.FindIndex(option => option == defaultOption);
    }

    protected void LoadFields()
    {
        BeamUserSettings settings = BeamMain.GetInstance().frontend.GetUserSettings();
        if (string.IsNullOrEmpty(settings.gameAcctAddr))
        {
            string addr;
            string keyStoreJson;
            (addr, keyStoreJson) = BeamMain.GetInstance().gameNet.NewCryptoAccountKeystore("password");
            settings.gameAcctAddr = addr;
            settings.gameAcctJSON.Add(addr, new PersistentAccount(PersistentAccount.AvailTypes.V3Keystore, addr, keyStoreJson).ToJson());
        }

        _SetupDropdown( p2pConnectionDrop.GetComponent<TMP_Dropdown>(),
            settings.p2pConnectionSettings.Keys.ToList(),
            settings.curP2pConnection
        );

        List<string> s = settings.blockchainInfos.Keys.ToList();
        s.Insert(0, kNoChainStr); // 1st item on list
        _SetupDropdown( blockchainDrop.GetComponent<TMP_Dropdown>(),
            s,
            settings.curBlockchain
        );

        anchorAddrField.GetComponent<TMP_InputField>().text = settings.anchorContractAddr;

        _SetupDropdown( gameAcctDrop.GetComponent<TMP_Dropdown>(),

            settings.gameAcctJSON.Keys.ToList(),
            settings.gameAcctAddr
        );

        permAcctField.GetComponent<TMP_InputField>().text = settings.permAcctAddr;
        netNameField.GetComponent<TMP_InputField>().text = settings.apianNetworkName;
        screenNameField.GetComponent<TMP_InputField>().text = settings.screenName;

        logLvlEditField.GetComponent<Toggle>().isOn = bool.Parse(settings.platformSettings.TryGetValue("enableLogLvlEdit", out var x) ? x : "false");

    }

    public void LoadAndShow()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings settings = mainObj.frontend.GetUserSettings();
        oldSettingsForCancel = new BeamUserSettings(settings); // COPY and stash in case cancel gets pressed
        LoadFields();
        moveOnScreen();
    }

    public void ResetToDefaults()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings defaults = BeamUserSettings.CreateDefault();
        mainObj.frontend.SetUserSettings(defaults);
        mainObj.uiController.ShowToast($"Default Settings", Toast.ToastColor.kGreen, 3);
        LoadFields();
    }

    public void SaveAndHide()
    {
        BeamMain mainObj = BeamMain.GetInstance();
        BeamUserSettings settings = mainObj.frontend.GetUserSettings();

        settings.curP2pConnection = p2pConnectionDrop.GetComponent<TMP_Dropdown>().captionText.text;

        string chainName = blockchainDrop.GetComponent<TMP_Dropdown>().captionText.text;
        settings.curBlockchain =  (chainName == kNoChainStr)? "" : chainName;
        mainObj.uiController.ShowToast($"curBlockchain: {settings.curBlockchain}", Toast.ToastColor.kGreen, 3);

        settings.anchorContractAddr = anchorAddrField.GetComponent<TMP_InputField>().text;

        settings.gameAcctAddr = gameAcctDrop.GetComponent<TMP_Dropdown>().captionText.text;
        settings.permAcctAddr = permAcctField.GetComponent<TMP_InputField>().text;

        settings.apianNetworkName = netNameField.GetComponent<TMP_InputField>().text;
        settings.screenName = screenNameField.GetComponent<TMP_InputField>().text;

        mainObj.platformSettings.enableLogLvlEdit = logLvlEditField.GetComponent<Toggle>().isOn; // unity-only setting

        mainObj.PersistSettings();
        mainObj.ApplyPlatformUserSettings();

        moveOffScreen();
    }

    public void CancelAndHide()
    {
         BeamMain mainObj = BeamMain.GetInstance();
        mainObj.uiController.ShowToast("Settings Cancelled", Toast.ToastColor.kGreen, 3);

        mainObj.frontend.SetUserSettings(oldSettingsForCancel);
        // setusersettings calls:
        //     mainObj.PersistSettings();
        //     mainObj.ApplyPlatformUserSettings();

        moveOffScreen();
    }

    protected void _copyToClipboard(string stringToCopy)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        WebGLBrowserClip.Copy(stringToCopy);
#endif
        GUIUtility.systemCopyBuffer = stringToCopy;
   }

    public void CopyPermAcctAddrToClipboard()
    {
        _copyToClipboard( permAcctField.GetComponent<TMP_InputField>().text);
        BeamMain.GetInstance().frontend.DisplayMessage(MessageSeverity.Info, "Permanent Account Copied to Clipboard");
    }

    public void CopyGameAcctAddrToClipboard()
    {
        _copyToClipboard(gameAcctDrop.GetComponent<TMP_Dropdown>().captionText.text);
        BeamMain.GetInstance().frontend.DisplayMessage(MessageSeverity.Info, "Game Account Copied to Clipboard");
    }


    public void CopyAnchorAddrToClipboard()
    {
        _copyToClipboard(anchorAddrField.GetComponent<TMP_InputField>().text);
        BeamMain.GetInstance().frontend.DisplayMessage(MessageSeverity.Info, "Anchor Contract Address Copied to Clipboard");
    }

}
