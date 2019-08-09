using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayerBike : Bike
{

    public override void DecideToTurn()
    {

    }

    public void FrobLeftButton()
    {
        _pendingTurn = TurnDir.kLeft;
    }

    public void FrobRightButton()
    {
        _pendingTurn = TurnDir.kRight;        
    }

    protected override Ground.Place DealWithPlace(Vector3 pos)
    {
        Ground.Place p = base.DealWithPlace(pos);
            
        //if (p != null && p.bike != this)
        //    Debug.Log(string.Format("Hit place ({0},{1}) TimeLeft: {2}", p.xIdx, p.zIdx, p.secsLeft));
        return p;
    }
}
