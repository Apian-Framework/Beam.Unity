using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class FeAiBike : FrontendBike
{
    public float kMaxBikeSeparation = Ground.gridSize * 6;
    public float turnTime = 2f;

    public float aiCheckTimeout = .7f;

    public float secsSinceLastAiCheck = 0;

    public float maxX = Ground.maxX - 10*Ground.gridSize; // assumes min === -max
    public float maxZ = Ground.maxZ - 10*Ground.gridSize;

    public override void DecideToTurn()
    {
        Vector2 pos = bb.position;
        BeamGameData gd = ((BeamGameInstance)be).gameData;
        Ground g = gd.Ground;

            secsSinceLastAiCheck += Time.deltaTime;   // TODO: be consistent with time
            if (secsSinceLastAiCheck > aiCheckTimeout) 
            {         
                secsSinceLastAiCheck = 0;
                // If not gonna turn maybe go towards the closest bike?
                if (pendingTurn == TurnDir.kUnset) {
                    Vector2 closestBikePos = ClosestBike(bb).position;
                    if ( Vector2.Distance(pos, closestBikePos) > kMaxBikeSeparation) // only if it's not really close
                        be.OnTurnRequested(bb.bikeId, TurnTowardsPos( closestBikePos, pos, heading )); 
                    else
                    {
                        bool doTurn = ( Random.value * turnTime <  GameTime.DeltaTime() );
                        if (doTurn)    
                            be.OnTurnRequested(bb.bikeId, (Random.value < .5f) ? TurnDir.kLeft : TurnDir.kRight);   
                    }               
                }

                // Do some looking ahead - maybe 
                Vector2 nextPos = UpcomingGridPoint(pos, heading);

                List<Vector2> othersPos = UpcomingEnemyPos(bb, 2); // up to 2 closest

                MoveNode moveTree = BuildMoveTree(nextPos, heading, 4, othersPos);
                List<dirAndScore> dirScores = TurnScores(moveTree);
                dirAndScore best =  SelectGoodTurn(dirScores); 
                if (  pendingTurn == TurnDir.kUnset || dirScores[(int)pendingTurn].score < best.score) 
                {
                    //Debug.Log(string.Format("New Turn: {0}", best.turnDir));                    
                    be.OnTurnRequested(bb.bikeId, best.turnDir);
                }
            }   

    }

    protected dirAndScore SelectGoodTurn(List<dirAndScore> dirScores) {
        int bestScore = dirScores.OrderBy( ds => ds.score).Last().score;
        // If you only take the best score you will almost always just go forwards.
        // But never select a 1 if there is anything better
        // &&& jkb - I suspect this doesn;t do exactly what I think it does.
        List<dirAndScore> turns = dirScores.Where( ds => (bestScore > 2) ? (ds.score > bestScore * .5) : (ds.score == bestScore)).ToList(); 
        int sel = (int)(Random.value * (float)turns.Count);
        //Debug.Log(string.Format("Possible: {0}, Sel Idx: {1}", turns.Count, sel));
        return turns[sel];
    }

}
