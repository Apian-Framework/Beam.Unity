using System.Collections;
using System.Collections.Generic;
using BeamBackend;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayStage : MonoBehaviour
{
	public float lookRadians = 1f; // positive is left
	public float lookDecayRate = 1f;	
	protected BeamMain _main = null;	

	// Use this for initialization
	protected void Start () 
	{		
		_main = BeamMain.GetInstance();		
	}

	protected void Update()
	{
		// TODO: This should use the new input eventhandler system,
		if (Input.GetKeyDown(KeyCode.Z ))
			OnViewLeftBtn();

		if (Input.GetKeyDown(KeyCode.X ))
			OnViewRightBtn();

		if (Input.GetKeyDown(KeyCode.Space ))
			OnViewUpBtn();

		if (Input.GetKeyDown(KeyCode.LeftArrow ))
			OnTurnLeftBtn();

		if (Input.GetKeyDown(KeyCode.RightArrow ))
			OnTurnRightBtn();			

		if (Input.GetKeyDown(KeyCode.S ))
			transform.Find("Scoreboard")?.SendMessage("toggle", null);

	}	

	// UI Button Handlers

	public void OnTurnLeftBtn() => _main.inputDispatch.LocalPlayerBikeLeft();

	public void OnTurnRightBtn() => _main.inputDispatch.LocalPlayerBikeRight();

	public void OnViewLeftBtn() =>_main.inputDispatch.LookAround(lookRadians, lookDecayRate); 

	public void OnViewRightBtn() => _main.inputDispatch.LookAround(-lookRadians, lookDecayRate);   

	public void OnViewUpBtn() => _main.inputDispatch.ToggleCamHeight();	

	public void OnRestartBtn() => _main.backend.RaiseRespawnPlayer();
}
