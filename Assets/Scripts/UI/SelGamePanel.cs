using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using BeamGameCode;
using Apian;

public class SelGamePanel : MovableUICanvasItem
{
    public GameObject newGameField;
    public GameObject existingGameDrop;
    public const string kNoGames = "No Games Found";

    protected IDictionary<string, BeamGameInfo> existingGames;

    protected BeamFrontend frontEnd;

    public void LoadAndShow(IDictionary<string, BeamGameInfo> existingGameDict)
    {
        existingGames = existingGameDict;
        frontEnd = BeamMain.GetInstance().frontend;

        // newGameField goes to default prompt. TODO: should there be a default/suggestion?
        // newGameField.GetComponent<TMP_InputField>().text = ???;
        TMP_Dropdown existingDrop = existingGameDrop.GetComponent<TMP_Dropdown>();

        existingDrop.ClearOptions();
        if (existingGames?.Count > 0)
        {
            existingDrop.AddOptions(existingGames.Keys.ToList<string>());
        }

        moveOnScreen();
    }

    public void DoJoinGame()
    {
        moveOffScreen();
        TMP_Dropdown drop = existingGameDrop.GetComponent<TMP_Dropdown>();
        frontEnd.logger.Info($"SelGamePanel.DoJoinGame()");
        BeamGameInfo selectedGame = existingGames[ drop.options[drop.value].text ];

        frontEnd.OnGameSelected(selectedGame, GameSelectedArgs.ReturnCode.kJoin );
    }

    public void DoCreateGame()
    {
        moveOffScreen();
        string newGameName = newGameField.GetComponent<TMP_InputField>().text;
        // TODO: THIS is AWFUL!!
        BeamGameInfo newGameInfo = frontEnd.beamAppl.beamGameNet.CreateBeamGameInfo(newGameName, LeaderSezGroupManager.groupType);

        frontEnd.OnGameSelected(newGameInfo, GameSelectedArgs.ReturnCode.kCreate );
    }

    public void DoCancel()
    {
        moveOffScreen();
        frontEnd.OnGameSelected(null, GameSelectedArgs.ReturnCode.kCancel);
    }

}
