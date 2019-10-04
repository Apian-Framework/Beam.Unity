using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class LocalPlayerBike : FrontendBike
{

    public override void Update()
    {
        base.Update();

         FeGround g = OldGameMain.GetInstance().feGround;
         if (_curTurn == TurnDir.kStraight) {
            // Vector3 pos = transform.position;
            // Vector3 otherPos = ClosestBike(this.gameObject).transform.position;

            //  TurnDir turnToZero = TurnTowardsPos( otherPos, pos, heading );

            //  Heading newHead = GameConstants.newHeadForTurn[(int)heading][(int)turnToZero];
            //  Debug.DrawLine( pos, pos + GameConstants.UnitOffsetForHeading(newHead]*Ground.gridSize, Color.white ); 


        //     //Vector3 offset = new Vector3(0,.1f,0);

        //     Vector3 nextPos = UpcomingGridPoint(pos, heading);
        //     //Debug.Log(string.Format("pos: {0}, next: {1}", pos, nextPos));
        //     Debug.DrawLine(pos, nextPos, Color.green );

        //     List<Ground.Place> places = PossiblePlacesForPointAndHeading(g, nextPos, heading);
        //     List<dirAndScore> dirCrashCnts = places.Select((p,idx) =>  new dirAndScore{turnDir = (Heading)idx, score = ( p == null ? 1 : 0)}).ToList();        
        //     foreach (dirAndScore dcc in dirCrashCnts)
        //     {
        //         Heading newHead = GameConstants.newHeadForTurn[(int)heading][(int)dcc.turnDir];
        //         Color c = dcc.score < 1 ? Color.red : Color.gray;
        //         Debug.DrawLine( nextPos, nextPos + GameConstants.UnitOffsetForHeading(newHead]*Ground.gridSize, c );                
        //     }
        }
    }


    public override void DecideToTurn()
    {

    }

    public void FrobLeftButton()
    {
        bb.TempSetPendingTurn(TurnDir.kLeft);
    }

    public void FrobRightButton()
    {
        bb.TempSetPendingTurn(TurnDir.kRight);
    }

}
