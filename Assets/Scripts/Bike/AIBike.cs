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

                // If not gonna turn maybe go towards the closest bike?
                if (_pendingTurn == TurnDir.kNone) {
                    Vector3 closestBikePos = ClosestBike(this.gameObject).transform.position;
                    if ( Vector3.Distance(pos, closestBikePos) > Ground.gridSize * 6) // only if it's not really close
                        _pendingTurn = TurnTowardsPos( closestBikePos, pos, heading );                
                }

                // If we're about to hit something, turn
                Vector3 nextPos = UpcomingGridPoint(pos, heading);

                // List<Vector3> pts = PossiblePointsForPointAndHeading( nextPos, heading);
                // List<dirAndScore> dirScores = pts.Select((pt,idx) =>  
                //     new dirAndScore{ turnDir = (TurnDir)idx, score = scoreForPoint(g, pt, g.GetPlace(pt))}).ToList();

                MoveNode moveTree = BuildMoveTree(nextPos, heading, 3);
                List<dirAndScore> dirScores = TurnScores(moveTree);
                dirAndScore best =  dirScores.OrderBy( ds => ds.score).Last(); // Add some randomness for ties?
                if ( dirScores[(int)_pendingTurn].score < best.score)
                  _pendingTurn =  best.turnDir;

 
            }
            
        }

        if (_pendingTurn == TurnDir.kNone) // TODO: differentiate between "not selected" and "straight"
        {
            bool doTurn = ( Random.value * turnTime <  GameTime.DeltaTime() );
            if (doTurn) {    
                _pendingTurn = (Random.value < .5f) ? TurnDir.kLeft : TurnDir.kRight;
            }
        }
    }


}
