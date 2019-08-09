using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDispatch : MonoBehaviour
{
    public LocalPlayerBike localPlayerBike {get; set; } = null;

    // Do domething better here
    public void LocalPlayerBikeLeft() => localPlayerBike.FrobLeftButton();
    public void LocalPlayerBikeRight() => localPlayerBike.FrobRightButton();    
}
