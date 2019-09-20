using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseBike
{
    public static readonly float length = 2.0f;
    public static readonly float speed =  15.0f;    
    public Player player {get; private set;} = null;
    public Vector2 position {get; private set;} = Vector2.zero; // always on the grid
    // NOTE: 2D position: x => east, y => north (in 3-space z is north and y is up)
    public Heading heading { get; private set;} = Heading.kNorth;

    //
    // Temporary stuff for refactoring
    //
    public TurnDir pendingTurn { get; private set;} = TurnDir.kUnset; // set and turn will start at next grid point

    public void TempSetPendingTurn(TurnDir d) => pendingTurn = d;

    public void TempSetHeading(Heading h) => heading = h;

    public BaseBike(Vector3 initialPos3)
    { 
        SetPos3(initialPos3); // probably don't need this, but it doesn;t hurt
    }

    public void Setup(Vector3 pos, Heading head, Player p)
    {
        p.Score = Player.kStartScore; // TODO: Change this!
        SetPos3(pos);
        heading = head;
        player = p;
    }

    public void SetPos3(Vector3 pos3)
    {
        position = new Vector2(pos3.x, pos3.z);
    }

    public Vector3 GetPos3(float height = 0)
    {
        return new Vector3(position.x, height, position.y);
    }

    public void DoUpdate(float secs)
    {
        _updatePosition(secs);
    }

    private void _updatePosition(float secs)
    {
        Vector2 upcomingPoint = UpcomingGridPoint(this, Ground.gridSize);
        float timeToPoint = Vector2.Distance(position, upcomingPoint) / speed;

        Vector2 newPos = position;
        Heading newHead = heading;

        if (secs >= timeToPoint) 
        {
            DoAtGridPoint(upcomingPoint, heading);
            secs -= timeToPoint;
            newPos =  upcomingPoint;
            newHead = GameConstants.NewHeadForTurn(heading, pendingTurn);
            pendingTurn = TurnDir.kUnset;
        }
        newPos += GameConstants.UnitOffset2ForHeading(heading) * secs * speed;

        position = newPos;
        heading = newHead;
    }

    protected virtual void DoAtGridPoint(Vector2 pos, Heading head)
    {

    }

    //
    // Static tools. Potentially useful publicly
    // 
    public static Vector2 NearestGridPoint(BaseBike bb, float gridSize)
    {
        float invGridSize = 1.0f / gridSize;
        return new Vector2(Mathf.Round(bb.position.x * invGridSize) * gridSize, Mathf.Round(bb.position.y * invGridSize) * gridSize);
    }

    public static Vector2 UpcomingGridPoint(BaseBike bb, float gridSize)
    {
        // it's either the current closest point (if direction to it is the same as heading)
        // or is the closest point + gridSize*unitOffsetForHeading[curHead] if closest point is behind us
        Vector2 point = NearestGridPoint(bb, gridSize);
        if (Vector2.Dot(GameConstants.UnitOffset2ForHeading(bb.heading), point - bb.position) < 0)
        {
            point += GameConstants.UnitOffset2ForHeading(bb.heading) * gridSize;
        }
        return point;
    }    



    


}
