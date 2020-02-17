using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;
using BikeControl;

public class FeRemoteBike : FrontendBike
{
    public override void Start()    
    {
        base.Start();
        isLocal = false;
    }
    protected override void CreateControl()
    {
        control = new RemoteControl();
    }
    public override void Update()
    {
        base.Update();
    }


}
