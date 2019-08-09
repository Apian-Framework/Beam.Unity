using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoBike : Bike
{
    public float turnTime = 2f;

    public override void DecideToTurn()
    {
        if (_pendingTurn == TurnDir.kNone)
        {
            bool doTurn = ( Random.value * turnTime <  GameTime.DeltaTime() );
            if (doTurn) {
                //_pendingTurn = TurnDir.kRight;                
                _pendingTurn = (Random.value < .5f) ? TurnDir.kLeft : TurnDir.kRight;
            }
        }
    }

}
