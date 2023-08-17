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
    public GameObject gameNameField;
    public GameObject agreeTypeField;
    public GameObject anchorAlgField;
    public GameObject anchorAddrField;
    public bool JoinAsValidator;

    public GameObject newGamePanelGo;
    public GameObject joinCreateButtonGo;

    public const string kNoGames = "No Games Found";

    protected IDictionary<string, BeamGameAnnounceData> existingGames;

    protected BeamGameInfo curGameInfo;
    protected bool createBeforeJoin; // if true, button says "create" else "join"

#if !SINGLE_THREADED
    protected TaskCompletionSource<GameSelectedEventArgs> completionSource;
#endif
    protected BeamFrontend frontEnd;

    protected string GameDisplayName(BeamGameAnnounceData gad)
    {
        return $"{gad.GameInfo.GameName} ({gad.GameInfo.GroupType})";   // GameOfFoo (LeaderSez)
    }

#if !SINGLE_THREADED
    public void LoadAndShowAsync(IDictionary<string, BeamGameAnnounceData> existingGameDict, TaskCompletionSource<GameSelectedEventArgs> tcs=null)
    {
        completionSource = tcs;
        _DoLoadAndShow(existingGameDict);
    }

    protected void NotifySelection(GameSelectedEventArgs selArgs)
    {
        if (completionSource != null)
            frontEnd.OnGameSelectedAsync(selArgs, completionSource);
        else
            frontEnd.OnGameSelected(selArgs);
    }
#else

    protected void NotifySelection( GameSelectedEventArgs selArgs)
    {
        frontEnd.OnGameSelected(selArgs);
    }

#endif

    public void LoadAndShow(IDictionary<string, BeamGameAnnounceData> existingGameDict)
    {
        _DoLoadAndShow(existingGameDict);
    }

    protected void _DoLoadAndShow(IDictionary<string, BeamGameAnnounceData> existingGameDict)
    {
        existingGames = existingGameDict;
        frontEnd = BeamMain.GetInstance().frontend;

        Dictionary<string, string> egTypesForCaptions = new Dictionary<string, string>();

        TMP_Dropdown existingDrop = existingGameDrop.GetComponent<TMP_Dropdown>();

        existingDrop.ClearOptions();
        if (existingGames?.Count > 0)
        {
            existingDrop.AddOptions(existingGames.Values.Select((entry) => GameDisplayName(entry)).ToList());
            _LoadCurrentGame(existingGames.Values.First().GameInfo, false);
        }
        else
        {
            existingDrop.AddOptions( new List<string>(){"(No existing games to join.)"});
            existingDrop.enabled = false;
            _LoadCurrentGame(null, true);
        }

        moveOnScreen();
    }

    protected void _LoadCurrentGame(BeamGameInfo gameInfo, bool createThenJoin)
    {
        BeamUserSettings settings = BeamMain.GetInstance().frontend.GetUserSettings();

        if (gameInfo == null)
        {
            gameNameField.GetComponent<TMP_InputField>().text = "";
            agreeTypeField.GetComponent<TMP_InputField>().text = "";
            anchorAlgField.GetComponent<TMP_InputField>().text = "";
            anchorAddrField.GetComponent<TMP_InputField>().text = "";
        } else {

            curGameInfo = gameInfo;
            createBeforeJoin = createThenJoin;
            joinCreateButtonGo.transform.Find("Text").GetComponent<TMP_Text>().text = createThenJoin ? "CREATE" : "JOIN"; // there's just 1 child

            gameNameField.GetComponent<TMP_InputField>().text = curGameInfo.GameName;
            agreeTypeField.GetComponent<TMP_InputField>().text = curGameInfo.GroupType;
            anchorAlgField.GetComponent<TMP_InputField>().text = curGameInfo.AnchorPostAlg;
            anchorAddrField.GetComponent<TMP_InputField>().text = curGameInfo.AnchorAddr;
        }
    }

    public void DoSetValidator(bool isSet)
    {
        JoinAsValidator = isSet;
    }

    public void DoJoinGame()
    {
        if (curGameInfo != null)
        {
            moveOffScreen();

            frontEnd.logger.Info($"SelGamePanel.DoJoinGame()");
            NotifySelection(
                new GameSelectedEventArgs(curGameInfo,
                    createBeforeJoin? GameSelectedEventArgs.ReturnCode.kCreate : GameSelectedEventArgs.ReturnCode.kJoin,
                    JoinAsValidator));
        }

    }

    // public void DoCreateGame() // SHould be DoCreateGameAsync
    // {
    //     moveOffScreen();
    //     string newGameName = newGameField.GetComponent<TMP_InputField>().text;
    //     string agreementType = agreeTypeDrop.GetComponent<TMP_Dropdown>().captionText.text;
    //     string anchorPostAlgorithm = anchorAlgDrop.GetComponent<TMP_Dropdown>().captionText.text;
    //     string anchorAddr = frontEnd.GetUserSettings().anchorContractAddr;

    //     BeamGameInfo newGameInfo = frontEnd.beamAppl.beamGameNet.CreateBeamGameInfo(newGameName, agreementType, anchorAddr, anchorPostAlgorithm, new GroupMemberLimits());
    //     NotifySelection(new GameSelectedEventArgs(newGameInfo, GameSelectedEventArgs.ReturnCode.kCreate, JoinAsValidator));
    // }

    public void OnExistingGameSelected()
    {
        TMP_Dropdown drop = existingGameDrop.GetComponent<TMP_Dropdown>();
        BeamGameInfo selectedGame = existingGames.Values.ToList()[drop.value].GameInfo;
        frontEnd.logger.Info($"SelGamePanel.OnExistingGameSelected()  Selected game: \"{selectedGame.GameName}\"");
        _LoadCurrentGame(selectedGame, false);
    }

    public void DoCreateNewGameASync()
    {
        NewGamePanel panel = newGamePanelGo.GetComponent<NewGamePanel>();
        panel.GetNewGame(this);
    }

    public void OnNewGameCreated(BeamGameInfo gameInfo)
    {
        frontEnd.logger.Info($"SelGamePanel.OnNewGameCreated()");
        _LoadCurrentGame(gameInfo, true);
    }

    public void DoCancel()
    {
        moveOffScreen();
        NotifySelection(new GameSelectedEventArgs(null, GameSelectedEventArgs.ReturnCode.kCancel, false));
    }

}
