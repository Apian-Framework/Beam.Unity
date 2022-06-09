using System.Collections;
using System.Collections.Generic;
using BeamGameCode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayStage : MonoBehaviour
{
	public float lookRadians = 1.57f; // positive is left
	public float lookDecayRate = .001f;
	protected BeamMain _main = null;

	protected PlayMenu _playMenu = null;
	protected Scoreboard _scoreboard;
	protected SelGamePanel _selGamePanel = null;

	// Use this for initialization
	protected void Start()
	{
		_main = BeamMain.GetInstance();
		_playMenu = (PlayMenu)utils.findObjectComponent("PlayMenu", "PlayMenu");
		_scoreboard = (Scoreboard)utils.findObjectComponent("Scoreboard", "Scoreboard");
		_selGamePanel = (SelGamePanel)utils.findObjectComponent("SelGamePanel", "SelGamePanel");
	}

	protected void OnEnable()
	{
		_playMenu?.moveOffScreenNow();
		_selGamePanel?.moveOffScreenNow();
		_scoreboard?.Reset();
	}

	protected void Update()
	{
		// TODO: This should use the new input eventhandler system,

		// First crack at new Input System
		if (Keyboard.current.zKey.wasPressedThisFrame)
			OnViewLeftBtn();

		if (Keyboard.current.xKey.wasPressedThisFrame)
			OnViewRightBtn();

		if (Keyboard.current.spaceKey.wasPressedThisFrame)
			OnViewUpBtn();

		if (Keyboard.current.sKey.wasPressedThisFrame)
			transform.Find("Scoreboard")?.SendMessage("toggle", null);

		if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
			OnTurnLeftBtn();

		if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
			OnTurnRightBtn();



	}

	// UI Button Handlers

	public void OnTurnLeftBtn() => _main.inputDispatch.LocalPlayerBikeLeft();

	public void OnTurnRightBtn() => _main.inputDispatch.LocalPlayerBikeRight();

	public void OnViewLeftBtn() =>_main.inputDispatch.LookAround(lookRadians, lookDecayRate);

	public void OnViewRightBtn() => _main.inputDispatch.LookAround(-lookRadians, lookDecayRate);

	public void OnViewUpBtn() => _main.inputDispatch.SwitchCameraView();

	public void OnRestartBtn() => _main.beamApp.mainAppCore.RaiseRespawnPlayer();

	public void OnExitBtn()
	{
		if (_main.beamApp.CurrentGameModeId() == BeamModeFactory.kPractice)
			_main.beamApp.OnSwitchModeReq(BeamModeFactory.kSplash, null);
		else
			_main.beamApp.OnPopModeReq(null);
	}


}
