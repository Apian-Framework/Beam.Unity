using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using BeamGameCode;
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
			peers.Values.OrderBy(p => p.PeerAddr).Select(p =>  $"{SID(p.PeerAddr)} - {p.Name}\n")
		);

	}

	protected void updateGameList(Dictionary<string, BeamGameAnnounceData> games)
	{
		GameListFld.text = string.Join("",
			games.Values.OrderBy( g => g.GameInfo.GameName).Select( g =>
				$"Name: {g.GameInfo.GameName} - {g.GameInfo.GroupId}\n"
			  + $"  Type: {g.GameInfo.GroupType} Peers: {g.GameStatus.ActiveMemberCount}\n"
			)
		);

	}

}
