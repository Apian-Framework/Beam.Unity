using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class FeAiBike : FrontendBike
{
    protected override void CreateControl()
    {
        Debug.Log(string.Format("FeAiBike.CreateControl()"));        
        control = new AiControl();
    }
}
