using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamGameCode;
using BikeControl;

public class FePlayerBike : FrontendBike
{
    public override void Start()
    {
        base.Start();
        engineSound.volume = .75f;
    }

    protected override void CreateControl()
    {
        control = new PlayerControl();
    }
    public override void Update()
    {
        base.Update();

        // float dbgLineY = .1f;
        // BeamGameData gd = BeamMain.GetInstance().backend.gameData;
        // if (_curTurn == TurnDir.kStraight || (_curTurn == TurnDir.kUnset)) {

        //     BaseBike bBike = bb as BaseBike;
        //     // //
        //     // // Draw direction to turn to get to closest bike
        //     // //

        //     // Vector2 pos2 = bb.position;
        //     // Vector2 otherPos2 = gd.ClosestBike(this.bb).position;

        //     // TurnDir turnToZero = BikeUtils.TurnTowardsPos( otherPos2, pos2, heading );

        //     // Heading newHead = GameConstants.NewHeadForTurn(heading,turnToZero);
        //     // Debug.DrawLine( utils.Vec3(pos2, dbgLineY), utils.Vec3(pos2 + (GameConstants.UnitOffset2ForHeading(newHead)*Ground.gridSize), dbgLineY), Color.white );

        //     //
        //     // Draw some other crap
        //     //

        //     //  Vector2 pos2 = bb.position;
        //     //  Vector2 nextPos2 =  bBike.UpcomingGridPoint();
        //     //  Debug.DrawLine(utils.Vec3(pos2), utils.Vec3(nextPos2), Color.green );

        //     // List<Vector2> othersPos = gd.CloseBikePositions(bb, 2); // up to 2 closest

        //     // BikeUtils.MoveNode moveTree = BikeUtils.BuildMoveTree(gd.Ground, nextPos2, heading, 4, othersPos);
        //     // List<DirAndScore> dirScores = BikeUtils.TurnScores(moveTree);
        //     // foreach (DirAndScore dcc in dirScores)
        //     // {
        //     //     Heading newHead = GameConstants.NewHeadForTurn(heading,dcc.turnDir);
        //     //     Color c = dcc.score < 5 ? Color.red : Color.green;
        //     //     Debug.DrawLine( utils.Vec3(nextPos2), utils.Vec3(nextPos2 + GameConstants.UnitOffset2ForHeading(newHead)*Ground.gridSize), c );
        //     // }
        // }
    }

    public void RequestTurn(TurnDir dir)
    {
        control.RequestTurn(dir, true); // allow deferred
    }

}
