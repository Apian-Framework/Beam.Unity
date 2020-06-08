using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;
using BikeControl;

public class FeRemoteBike : FrontendBike
{
    public override void Awake()
    {
        base.Awake(); // when object is created
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
