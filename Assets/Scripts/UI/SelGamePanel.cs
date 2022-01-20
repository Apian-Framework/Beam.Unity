using System.Collections;
using System.Collections.Generic;

using System.Linq;
using UnityEngine;
using TMPro;
using BeamGameCode;
using Apian;

#if !SINGLE_THREADED
using System.Threading;
using System.Threading.Tasks;
#endif

public class SelGamePanel : MovableUICanvasItem
{
    public GameObject existingGameDrop;
    public GameObject newGameField;
    public GameObject agreeTypeDrop;
    public const string kNoGames = "No Games Found";

    protected IDictionary<string, BeamGameAnnounceData> existingGames;

#if !SINGLE_THREADED
    protected TaskCompletionSource<GameSelectedEventArgs> completionSource;
#endif
    protected BeamFrontend frontEnd;

    protected string GameDisplayName(BeamGameAnnounceData gad)
    {
        return $"{gad.GameInfo.GameName} ({gad.GameInfo.GroupType})";   // GameOfFoo (LeaderSez)
    }

#if !SINGLE_THREADED
    public void LoadAndShow(IDictionary<string, BeamGameAnnounceData> existingGameDict, TaskCompletionSource<GameSelectedEventArgs> tcs=null)
    {
        completionSource = tcs;
        existingGames = existingGameDict;
        frontEnd = BeamMain.GetInstance().frontend;

        TMP_Dropdown typeDrop = agreeTypeDrop.GetComponent<TMP_Dropdown>();
        typeDrop.ClearOptions();
        typeDrop.AddOptions( BeamApianFactory.ApianGroupTypes);

        Dictionary<string, string> egTypesForCaptions = new Dictionary<string, string>();

        TMP_Dropdown existingDrop = existingGameDrop.GetComponent<TMP_Dropdown>();

        existingDrop.ClearOptions();
        if (existingGames?.Count > 0)
        {
            existingDrop.AddOptions(existingGames.Values.Select((entry) => GameDisplayName(entry)).ToList());
        }
        else
        {
            existingDrop.AddOptions( new List<string>(){"(No existing games to join.)"});
            existingDrop.enabled = false;
        }

        moveOnScreen();
    }

    public void DoJoinGame() // SHould be DoJoinGameAsync()
    {
        moveOffScreen();
        TMP_Dropdown drop = existingGameDrop.GetComponent<TMP_Dropdown>();
        frontEnd.logger.Info($"SelGamePanel.DoJoinGame()");
        BeamGameInfo selectedGame = existingGames.Values.ToList()[drop.value].GameInfo;
        frontEnd.OnGameSelected(new GameSelectedEventArgs(selectedGame, GameSelectedEventArgs.ReturnCode.kJoin), completionSource);
    }


    public void DoCreateGame() // SHould be DoCreateGameAsync
    {
        moveOffScreen();
        string newGameName = newGameField.GetComponent<TMP_InputField>().text;
        string agreementType = agreeTypeDrop.GetComponent<TMP_Dropdown>().captionText.text;

        BeamGameInfo newGameInfo = frontEnd.beamAppl.beamGameNet.CreateBeamGameInfo(newGameName, agreementType);

        frontEnd.OnGameSelected(new GameSelectedEventArgs(newGameInfo, GameSelectedEventArgs.ReturnCode.kCreate), completionSource );
    }

    public void DoCancel()
    {
        moveOffScreen();
        frontEnd.OnGameSelected(new GameSelectedEventArgs(null, GameSelectedEventArgs.ReturnCode.kCancel), completionSource);
    }

#endif


}
