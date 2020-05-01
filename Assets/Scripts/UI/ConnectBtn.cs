using UnityEngine;
using System.Collections;
using BeamBackend;

public class ConnectBtn : UIBtn  {

	protected BeamMain _main = null;

	// Use this for initialization
	protected override void Start ()
	{
		base.Start();
		_main = BeamMain.GetInstance();

	}

	protected void _DoConnect()
	{
		_main.core.OnSwitchModeReq(BeamModeFactory.kConnect, null);
	}

	protected override void Update()
	{
		base.Update();
		if (Input.GetKeyDown(KeyCode.Return))
			_DoConnect();

	}

	public override void doSelect()
	{
		_DoConnect();
	}
}

