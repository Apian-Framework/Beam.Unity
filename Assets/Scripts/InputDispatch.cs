using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputDispatch : MonoBehaviour
{
    public FePlayerBike localPlayerBike {get; private set; } = null;

    public void SetLocalPlayerBike(GameObject playerBike)
    {
       localPlayerBike = playerBike.transform.GetComponent<FePlayerBike>();
    }

    // Do domething better here
    public void LocalPlayerBikeLeft() => localPlayerBike.FrobLeftButton();
    public void LocalPlayerBikeRight() => localPlayerBike.FrobRightButton();    
}
