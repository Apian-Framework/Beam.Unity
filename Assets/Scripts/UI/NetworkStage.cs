using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using BeamGameCode;
using P2pNet;
using UnityEngine;
using UniLog;
using static UniLog.UniLogger; // for SID()

public class NetworkStage : MonoBehaviour
{
	public GameObject ConnectingTxt;
	public GameObject ProceedBtn;
	public TMP_Text NetworkNameTxt;
	public TMP_Text PeerCntTxt;
	public TMP_InputField PeerListFld; // is set to non-interactive
	public TMP_Text GameCntTxt;
	public TMP_InputField GameListFld; // is set to non-interactive
	public TMP_Text CancelBtnTxt;

	protected BeamMain _main = null;

	// Use this for initialization
	protected void Start ()
	{
		_main = BeamMain.GetInstance();
	}

	public void OnProceedBtn()
	{
		_main.beamApp.OnPushModeReq(BeamModeFactory.kNetPlay, null);
	}

	public void OnCancelBtn()
	{
		_main.beamApp.OnSwitchModeReq(BeamModeFactory.kSplash, null);
	}

	public void ShowProceedButton(bool showIt=true)
	{
		ProceedBtn.SetActive(showIt);
		ConnectingTxt.SetActive(!showIt);
		CancelBtnTxt.text = showIt ? "DISCONNECT" : "CANCEL";

	}

	public void OnNetUpdate(BeamNetInfo netInfo)
	{
		NetworkNameTxt.text = netInfo.NetName;
		PeerCntTxt.text = netInfo.PeerCount.ToString();
		GameCntTxt.text = netInfo.GameCount.ToString();

		updatePeerList(netInfo.BeamPeers);
		updateGameList(netInfo.BeamGames);

	}

	protected void updatePeerList(Dictionary<string, BeamNetworkPeer> peers)
	{
		PeerListFld.text = string.Join("",
			peers.Values.OrderBy(p => p.PeerAddr).Select(p =>
			{
            	PeerNetworkStats stats = _main.beamApp.beamGameNet.GetPeerNetStats(p.PeerAddr);
				long lagMs = stats?.NetLagMs == null ? 0 : stats.NetLagMs;
				string lagStr = $"{(lagMs==0?"":" Lag: "+stats?.NetLagMs)}"; // Don;t display 0 lag
				string localStr = $"{(p.PeerAddr==_main.beamApp.LocalPeer.PeerAddr?"(L) ":"")}";

				return $"{localStr}{p.Name} ({SID(p.PeerAddr)}) {lagStr}\n";
			})
		);

	}

	protected void updateGameList(Dictionary<string, BeamGameAnnounceData> games)
	{
		GameListFld.text = string.Join("",
			games.Values.OrderBy( g => g.GameInfo.GameName).Select( ga => {
				BeamGameInfo gi = ga.GameInfo;
				BeamGameStatus gs = ga.GameStatus;
            	string gameId = $"{gi.GameName}: {gi.GroupType}";
            	string memberInf = $"{gs.MemberCount} ({gi.MemberLimits.MinMembers}/{gi.MemberLimits.MaxMembers})";
            	string playerInf = $"{gs.PlayerCount} ({gi.MemberLimits.MinPlayers}/{gi.MemberLimits.MaxPlayers}";
            	string validatorInf = $"{gs.ValidatorCount} ({gi.MemberLimits.MinValidators}/{gi.MemberLimits.MaxValidators})";
            	//return$" {gameId}, Members: {memberInf}, Players: {playerInf}, Validators: {validatorInf}";
				return$" {gameId}  Players: {gs.PlayerCount}, Validators: {gs.ValidatorCount}\n";
			})
		);

	}

}
