using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Bike : MonoBehaviour
{
    protected class dirAndScore { public TurnDir turnDir; public int score; };

    public BaseBike bb = null;

    // Stuff that really lives in BaseBike. 
    // TODO: maybe get rid of this? Or maybe it's ok    
    public Player player { get => bb.player; } 
    public Heading heading { get => bb.heading; }
    
    public float turnRadius = 1; // 2.0f;

    public float maxLean = 50.0f;

    protected GameObject ouchObj;

    protected static readonly float[] turnStartTheta = {
        90f, 180f, 270f, 0f
    };

    public static readonly float kRoot2Over2 = 2.0f / Mathf.Sqrt(2);

    public TurnDir _pendingTurn = TurnDir.kUnset; // set and turn will start at next grid point

    Vector3 _turnAxis = Vector3.up; // just a dummy location
    float _turnStartTheta = 0f;
    float _turnTheta = 0f; // 0 is north, increases clockwise
    protected TurnDir _curTurn = TurnDir.kStraight;

    public virtual void Setup(Vector3 pos, Heading head, Player p)
    {
        bb = new BaseBike(transform.position);
        bb.Setup(pos, head, p);
        transform.position = pos;
        SetColor(p.Team.Color);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize pointing direction based on heading;
        // assumes pos and heading are set on creation
        Vector3 angles = transform.eulerAngles;
        angles.z = 0;
        angles.y = turnStartTheta[(int)heading] - 90f;
        transform.eulerAngles = angles;

        ouchObj = transform.Find("Ouch").gameObject;
        ouchObj.SetActive(false);
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Vector3 pos = transform.position;
        Vector3 angles = transform.eulerAngles;
        float deltaT = GameTime.DeltaTime();

        DecideToTurn();

        Vector3 gridPt = NearestGridPoint(pos, Ground.gridSize);
        bool prevAtGridPt = AtGridPoint(gridPt, pos, BaseBike.length, turnRadius); // see if this frame's motion changes the status

        // 
        // Do frame motion
        //
        if (_curTurn != TurnDir.kStraight)
        {
            // TODO: be consistent with degrees/radians
            // Do a turn iteration
            float dThetaRad = (_curTurn == TurnDir.kLeft ? -1f : 1f) * deltaT * BaseBike.speed / turnRadius;
            _turnTheta += dThetaRad * Mathf.Rad2Deg;
            float theta = _turnStartTheta + _turnTheta;
            pos = _turnAxis + new Vector3(turnRadius * Mathf.Sin(theta * Mathf.Deg2Rad), 0, turnRadius * Mathf.Cos(theta * Mathf.Deg2Rad));
            //Debug.Log(string.Format("startTheta: {0}", _turnStartTheta));             
            //Debug.Log(string.Format("turnTheta: {0}", _turnTheta)); 

            if (Mathf.Abs(_turnTheta) >= 90)
            {
                _curTurn = TurnDir.kStraight;
                angles.z = 0;
                angles.y = turnStartTheta[(int)heading] - 90f;
            }
            else
            {
                angles.z = -Mathf.Sin(_turnTheta * 2.0f * Mathf.Deg2Rad) * maxLean;
                angles.y = theta + (_curTurn == TurnDir.kLeft ? -1f : 1f) * 90f;
            }
            transform.eulerAngles = angles;

        }
        else
        {
            pos += GameTime.DeltaTime() * BaseBike.speed * GameConstants.UnitOffset3ForHeading(heading);
        }

        // Deal with this frame's motion

        bool atGridPt = AtGridPoint(gridPt, pos, BaseBike.length, turnRadius);

        if (atGridPt)
        {
            if (!prevAtGridPt)
            {
                // Just crossed onto a grid point
                DealWithPlace(pos);

                // Waiting to turn?
                if (!prevAtGridPt && _pendingTurn != TurnDir.kStraight && _pendingTurn != TurnDir.kUnset && _curTurn == TurnDir.kStraight)
                {
                    // should be StartTurn()
                    Heading prevHead = heading;
                    _curTurn = _pendingTurn;
                    bb.TempSetHeading(GameConstants.NewHeadForTurn(heading, _pendingTurn));

                    // set up turn axis and angle
                    _turnAxis = gridPt - (GameConstants.UnitOffset3ForHeading(prevHead) - GameConstants.UnitOffset3ForHeading(heading)) * turnRadius;
                    _turnStartTheta = turnStartTheta[(int)prevHead] + (_curTurn == TurnDir.kRight ? 180f : 0f);
                    _turnTheta = 0;
                    //Debug.Log(string.Format("gridPt: {0}", gridPt));             
                    //Debug.Log(string.Format("turnAxis: {0}", _turnAxis));                
                }

                _pendingTurn = TurnDir.kUnset;  // reset when you get to a grid point        
            }
        }

        transform.position = pos;

    }


    public virtual void DecideToTurn() { }


    public void SetColor(Color newC)
    {
        transform.Find("BikeMesh").GetComponent<Renderer>().material.color = newC;
        transform.Find("Trail").GetComponent<Renderer>().material.SetColor("_EmissionColor", newC);
    }

    protected virtual Ground.Place DealWithPlace(Vector3 pos) // returns place in case override wants to do something with it.
    {
        Ground g = GameMain.GetInstance().ground;
        Ground.Place p = g.GetPlace(pos);
        if (p == null)
        {
            p = g.ClaimPlace(this, pos);
            if (p == null)
                GameMain.GetInstance().ReportScoreEvent(this, ScoreEvent.kOffMap, null);
            else
                GameMain.GetInstance().ReportScoreEvent(this, ScoreEvent.kClaimPlace, null);
        }
        else
        {
            ouchObj.SetActive(false); // restart in case the anim is already running
            ouchObj.SetActive(true);
            GameMain.GetInstance().ReportScoreEvent(this, p.bike.player.Team == player.Team ? ScoreEvent.kHitFriendPlace : ScoreEvent.kHitEnemyPlace, p);
        }
        return p;
    }

    protected bool AtGridPoint(Vector3 gridPos, Vector3 bikePos, float bikeLen, float turnRadius)
    {
        // Assumes grid spacing is >= 2*turnRadius 
        float dist = Vector3.Distance(gridPos, bikePos);
        return dist <= turnRadius;
    }

    protected Vector3 NearestGridPoint(Vector3 pos, float gridSize)
    {
        float invGridSize = 1.0f / gridSize;
        return new Vector3(Mathf.Round(pos.x * invGridSize) * gridSize, pos.y, Mathf.Round(pos.z * invGridSize) * gridSize);
    }

    // Tools for AIs
    protected Vector3 UpcomingGridPoint(Vector3 curPos, Heading curHead)
    {
        // it's either the current closest point (if direction to it is the same as heading)
        // or is the closest point + gridSize*unitOffsetForHeading[curHead] if closest point is behind us
        Vector3 point = NearestGridPoint(curPos, Ground.gridSize);
        if (Vector3.Dot(GameConstants.UnitOffset3ForHeading(curHead), point - curPos) < 0)
        {
            point += GameConstants.UnitOffset3ForHeading(curHead) * Ground.gridSize;
        }
        return point;
    }

    protected static List<Vector3> PossiblePointsForPointAndHeading(Vector3 curPtPos, Heading curHead)
    {
        // returns a list of grid positions where you could go next if you are headed for one with the given heading
        // The entries correspond to turn directions (none, left, right) 
        // TODO use something like map() ?
        return new List<Vector3> {
            curPtPos + GameConstants.UnitOffset3ForHeading(GameConstants.NewHeadForTurn(curHead, TurnDir.kStraight))*Ground.gridSize,
            curPtPos + GameConstants.UnitOffset3ForHeading(GameConstants.NewHeadForTurn(curHead, TurnDir.kLeft))*Ground.gridSize,
            curPtPos + GameConstants.UnitOffset3ForHeading(GameConstants.NewHeadForTurn(curHead, TurnDir.kRight))*Ground.gridSize,
        };
    }

    protected List<Ground.Place> PossiblePlacesForPointAndHeading(Ground g, Vector3 curPtPos, Heading curHead)
    {
        return PossiblePointsForPointAndHeading(curPtPos, curHead).Select((pos) =>
           g.GetPlace(pos)).ToList();
    }

    protected TurnDir TurnTowardsPos(Vector3 targetPos, Vector3 curPos, Heading curHead)
    {
        Vector3 bearing = targetPos - curPos;
        float turnAngleDeg = Vector3.SignedAngle(bearing, GameConstants.UnitOffset3ForHeading(curHead), Vector3.up);
        //Debug.Log(string.Format("Pos: {0}, Turn Angle: {1}", curPos, turnAngleDeg));
        return turnAngleDeg > 45f ? TurnDir.kLeft : (turnAngleDeg < -45f ? TurnDir.kRight : TurnDir.kStraight);
    }

    protected GameObject ClosestBike(GameObject thisBike)
    {
        GameMain gm = GameMain.GetInstance();
        GameObject closest = gm.BikeList.Where(b => b != thisBike)
            .OrderBy(b => Vector3.Distance(b.transform.position, thisBike.transform.position)).First();
        return closest;
    }

    protected List<Vector3> UpcomingEnemyPos(GameObject thisBike, int maxCnt)
    {
        GameMain gm = GameMain.GetInstance();
        return gm.BikeList.Where(b => b != thisBike)
            .OrderBy(b => Vector3.Distance(b.transform.position, thisBike.transform.position)).Take(maxCnt) // gameObjects
            .Select(go => go.transform.position).ToList();
    }


    // AI Move stuff (here so player bike can display it)

    protected static int ScoreForPoint(Ground g, Vector3 point, Ground.Place place)
    {
        return g.PointIsOnMap(point) ? (place == null ? 5 : 1) : 0; // 5 pts for a good place, 1 for a claimed one, zero for off-map
    }

    protected MoveNode BuildMoveTree(Vector3 curPos, Heading curHead, int depth, List<Vector3> otherBadPos = null)
    {
        Ground g = GameMain.GetInstance().ground;
        Vector3 nextPos = UpcomingGridPoint(curPos, heading);
        MoveNode root = MoveNode.GenerateTree(g, nextPos, curHead, 1, otherBadPos);
        return root;
    }

    protected List<dirAndScore> TurnScores(MoveNode moveTree)
    {
        return moveTree.next.Select(n => new dirAndScore { turnDir = n.dir, score = n.BestScore() }).ToList();
    }


    public class MoveNode
    {
        public TurnDir dir; // the turn direction that got to here (index in parent's "next" list)
        public Vector3 pos;
        public Ground.Place place;
        public int score;
        public List<MoveNode> next; // length 3

        public MoveNode(Ground g, Vector3 p, Heading head, TurnDir d, int depth, List<Vector3> otherClaimedPos)
        {
            pos = p;
            dir = d; // for later lookup
            place = g.GetPlace(p);
            score = ScoreForPoint(g, pos, place);
            if (score == 0 && otherClaimedPos.Any(op => op.Equals(pos))) // TODO: make prettier
                score = 1; // TODO: use named scoring constants
            next = depth < 1 ? null : PossiblePointsForPointAndHeading(pos, head)
                    .Select((pt, childTurnDir) => new MoveNode(g,
                       pos + GameConstants.UnitOffset3ForHeading(GameConstants.NewHeadForTurn(head, (TurnDir)childTurnDir)) * Ground.gridSize,
                       head,
                       (TurnDir)childTurnDir,
                       depth - 1,
                       otherClaimedPos))
                    .ToList();
        }

        public static MoveNode GenerateTree(Ground g, Vector3 rootPos, Heading initialHead, int depth, List<Vector3> otherBadPos)
        {
            return new MoveNode(g, rootPos, initialHead, TurnDir.kStraight, depth, otherBadPos);
        }

        public int BestScore()
        {
            // Express this trivially until I understand th epossibilities
            if (score == 0) // This kills you, why look further?
                return 0;

            if (next == null) // no kids
                return score;

            // return score + next.Select( n => n.BestScore()).OrderBy(i => i).Last();
            return score + next.Select(n => n.BestScore()).Where(i => i > 0).Sum();
        }

    }

}
