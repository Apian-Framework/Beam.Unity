using System.Collections;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;
using TMPro;
using BeamGameCode;
using Apian;

using System.Threading;
using System.Threading.Tasks;

public class NewGamePanel : MovableUICanvasItem
{
    public GameObject gameNameField;
    public GameObject agreeTypeDrop;
    public GameObject anchorAddrField;
    public GameObject anchorAlgDrop;

    protected TaskCompletionSource<GameSelectedEventArgs> completionSource;
    protected BeamFrontend frontEnd;
    protected SelGamePanel selGamePanel;

    public void GetNewGame(SelGamePanel _selGamePanel)
    {
        selGamePanel = _selGamePanel;
        _DoLoadAndShow();
    }

    protected void _DoLoadAndShow()
    {
        frontEnd = BeamMain.GetInstance().frontend;

        // GameNameield is uninitialized

        TMP_Dropdown typeDrop = agreeTypeDrop.GetComponent<TMP_Dropdown>();
        typeDrop.ClearOptions();
        typeDrop.AddOptions( BeamApianFactory.ApianGroupTypes.Where(type => type != SinglePeerGroupManager.kGroupType).ToList());

        anchorAddrField.GetComponent<TMP_InputField>().text = frontEnd.GetUserSettings().anchorContractAddr;

        TMP_Dropdown algDrop = anchorAlgDrop.GetComponent<TMP_Dropdown>();
        algDrop.ClearOptions();
        algDrop.AddOptions( ApianGroupInfo.AnchorPostAlgorithms.ToList());

        OnAnchorTypeSel(); // show/hide the address field

        moveOnScreen();
    }

    public void OnAnchorTypeSel()
    {
        // enable/disable anchor address field (index 0 is "none")
        int selAnchorIdx =  anchorAlgDrop.GetComponent<TMP_Dropdown>().value;
        anchorAddrField.SetActive( selAnchorIdx != 0);

    }

    public void DoCreateGame() // SHould be DoCreateGameAsync
    {
        moveOffScreen();
        string newGameName = gameNameField.GetComponent<TMP_InputField>().text;
        string agreementType = agreeTypeDrop.GetComponent<TMP_Dropdown>().captionText.text;
        string anchorPostAlgorithm = anchorAlgDrop.GetComponent<TMP_Dropdown>().captionText.text;
        string anchorAddr = "";

        if ( anchorAlgDrop.GetComponent<TMP_Dropdown>().value != 0) {
            anchorAddr =  anchorAddrField.GetComponent<TMP_InputField>().text;
            frontEnd.GetUserSettings().anchorContractAddr = anchorAddr; // if there's an anchor, write its addr to settings
        }

        BeamGameInfo newGameInfo = frontEnd.beamAppl.beamGameNet.CreateBeamGameInfo(newGameName, agreementType, anchorAddr, anchorPostAlgorithm, new GroupMemberLimits());
        selGamePanel.OnNewGameCreated(newGameInfo);
    }

    public void DoCancel()
    {
        moveOffScreen();
        selGamePanel.OnNewGameCreated(null);
    }



}
