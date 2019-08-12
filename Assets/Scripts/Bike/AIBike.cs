using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBike : Bike
{
    public float turnTime = 2f;

    public float maxX = Ground.maxX - 10*Ground.gridSize; // assumes min === -max
    public float maxZ = Ground.maxZ - 10*Ground.gridSize;

    public override void DecideToTurn()
    {
        Vector3 pos = transform.position;

        if (_pendingTurn == TurnDir.kNone)
        {
            bool doTurn = ( Random.value * turnTime <  GameTime.DeltaTime() );

            // Going to run off the world? Stupid tests.
            if (   (pos.x > maxX && heading != Heading.kWest) 
                || (pos.x < -maxX && heading != Heading.kEast)
                || (pos.z > maxZ && heading != Heading.kSouth)
                || (pos.z < -maxZ && heading == Heading.kNorth) )
            {
                // No, this doesn't guarantee anything
                doTurn = true;
            }

            if (doTurn) {    
                _pendingTurn = (Random.value < .5f) ? TurnDir.kLeft : TurnDir.kRight;
            }
        }
    }

}
