using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AIBike : Bike
{
    public float turnTime = 2f;

    public float aiCheckTimeout = .25f;

    public float secsSinceLastAiCheck = 0;

    public float maxX = Ground.maxX - 10*Ground.gridSize; // assumes min === -max
    public float maxZ = Ground.maxZ - 10*Ground.gridSize;

    public override void DecideToTurn()
    {
        Vector3 pos = transform.position;

        Ground g = GameMain.GetInstance().ground;

        if (_curTurn == TurnDir.kNone) { // not currently turning
            secsSinceLastAiCheck += Time.deltaTime;   
            if (secsSinceLastAiCheck > aiCheckTimeout) {         
                secsSinceLastAiCheck = 0;
                    Vector3 nextPos = UpcomingGridPoint(pos, heading);
                List<Ground.Place> places = PossiblePlacesForPointAndHeading(g, nextPos, heading);
                List<dirAndScore> dirScores = places.Select((p,idx) =>  new dirAndScore{turnDir = (TurnDir)idx, score = ( p == null ? 1 : 0)}).ToList();
                if ( dirScores[(int)_pendingTurn].score < 1) {
                    _pendingTurn =  dirScores.OrderBy( ds => ds.score).Last().turnDir; // // TODO: add some randomness to ties?
                }
            }
        }

        if (_pendingTurn == TurnDir.kNone) // TODO: differentiate between "not selected" and "straight"
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
