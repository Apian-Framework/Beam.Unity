using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BeamGameCode;


public class SelGamePanel : MovableUICanvasItem
{
    public GameObject newGameField;
    public GameObject existingGameDrop;
    public const string kNoGames = "No Games Found";

    protected BeamFrontend frontEnd;

    public void LoadAndShow(List<string> existingGames)
    {
        frontEnd = BeamMain.GetInstance().frontend;

        // newGameField goes to default prompt. TODO: should there be a default/suggestion?
        // newGameField.GetComponent<TMP_InputField>().text = ???;
        TMP_Dropdown existingDrop = existingGameDrop.GetComponent<TMP_Dropdown>();

        existingDrop.ClearOptions();
        if (existingGames?.Count > 0)
        {
            existingDrop.AddOptions(existingGames);
        }

        moveOnScreen();
    }

    public void DoJoinGame()
    {
        moveOffScreen();
        TMP_Dropdown drop = existingGameDrop.GetComponent<TMP_Dropdown>();

        string newGameName = drop.options[drop.value].text;

        frontEnd.OnGameSelected(newGameName, GameSelectedArgs.ReturnCode.kJoin );
    }

    public void DoCreateGame()
    {
        moveOffScreen();
        string newGameName = newGameField.GetComponent<TMP_InputField>().text;
        frontEnd.OnGameSelected(newGameName, GameSelectedArgs.ReturnCode.kCreate );
    }

    public void DoCancel()
    {
        moveOffScreen();
        frontEnd.OnGameSelected(null, GameSelectedArgs.ReturnCode.kCancel);
    }

}
