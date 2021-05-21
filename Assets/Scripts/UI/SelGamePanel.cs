using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using UnityEngine;
using TMPro;
using BeamGameCode;
using Apian;

public class SelGamePanel : MovableUICanvasItem
{
    public GameObject existingGameDrop;
    public GameObject newGameField;
    public GameObject agreeTypeDrop;
    public const string kNoGames = "No Games Found";

    protected IDictionary<string, BeamGameInfo> existingGames;
    protected CancellationTokenSource canceller;

    protected BeamFrontend frontEnd;

    public void LoadAndShow(IDictionary<string, BeamGameInfo> existingGameDict, CancellationTokenSource cts=null)
    {
        canceller = cts;
        existingGames = existingGameDict;
        frontEnd = BeamMain.GetInstance().frontend;

        TMP_Dropdown typeDrop = agreeTypeDrop.GetComponent<TMP_Dropdown>();
        typeDrop.ClearOptions();
        typeDrop.AddOptions( new List<string>(){LeaderSezGroupManager.kGroupType});

        // newGameField goes to default prompt. TODO: should there be a default/suggestion?
        // newGameField.GetComponent<TMP_InputField>().text = ???;
        TMP_Dropdown existingDrop = existingGameDrop.GetComponent<TMP_Dropdown>();

        existingDrop.ClearOptions();
        if (existingGames?.Count > 0)
        {
            existingDrop.AddOptions(existingGames.Keys.ToList<string>());
        }
        else
        {
            existingDrop.AddOptions( new List<string>(){"(No existing games to join.)"});
            existingDrop.enabled = false;
        }

        moveOnScreen();
    }

    public void DoJoinGame()
    {
        moveOffScreen();
        TMP_Dropdown drop = existingGameDrop.GetComponent<TMP_Dropdown>();
        frontEnd.logger.Info($"SelGamePanel.DoJoinGame()");
        BeamGameInfo selectedGame = existingGames[ drop.options[drop.value].text ];
        frontEnd.OnGameSelected(new GameSelectedArgs(selectedGame, GameSelectedArgs.ReturnCode.kJoin), canceller);
    }

    public void DoCreateGame()
    {
        moveOffScreen();
        string newGameName = newGameField.GetComponent<TMP_InputField>().text;
        string agreementType = agreeTypeDrop.GetComponent<TMP_Dropdown>().captionText.text;

        BeamGameInfo newGameInfo = frontEnd.beamAppl.beamGameNet.CreateBeamGameInfo(newGameName, agreementType);

        frontEnd.OnGameSelected(new GameSelectedArgs(newGameInfo, GameSelectedArgs.ReturnCode.kCreate), canceller );
    }

    public void DoCancel()
    {
        moveOffScreen();
        frontEnd.OnGameSelected(new GameSelectedArgs(null, GameSelectedArgs.ReturnCode.kCancel), canceller);
    }

}
