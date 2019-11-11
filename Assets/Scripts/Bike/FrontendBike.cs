using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class FrontendBike : MonoBehaviour
{
    public IBike bb = null;
    protected IBeamBackend be = null;

    // Stuff that really lives in backend. 
    // TODO: maybe get rid of this? Or maybe it's ok    
    public Player player { get => bb.player; } 
    public Heading heading { get => bb.heading; }
    
    // Temp (probably) stuff for refactoring to add BaseBike
    public TurnDir pendingTurn { get => bb.pendingTurn; }

    public float turnRadius = 1.5f;

    public float maxLean = 40.0f;

    protected GameObject ouchObj;

    protected static readonly float[] turnStartTheta = {
        90f, 180f, 270f, 0f
    };

    protected TurnDir _curTurn = TurnDir.kStraight;
    protected Vector2 _curTurnPt; // only has meaning when curTuen is set
    protected Vector2 _curTurnCenter; // only has meaning when curTuen is set 
    protected float _curTurnStartTheta;   

    // Important: Setup() is not called until after Awake() and Start() have been called on the
    // GameObject and components. Both of those are called when the GO is instantiated
    public virtual void Setup(IBike beBike, IBeamBackend backEnd)
    {
        be = backEnd;
        bb = beBike;
        transform.position = utils.Vec3(bb.position); // Is probably already set to this
        SetColor(utils.hexToColor(bb.player.Team.Color));    
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
    // public virtual void OldUpdate()
    // {
    //     Vector3 pos = transform.position;
    //     Vector3 angles = transform.eulerAngles;
    //     float deltaT = GameTime.DeltaTime();

    //     DecideToTurn();

    //     Vector3 gridPt = NearestGridPoint(pos, Ground.gridSize);
    //     bool prevAtGridPt = AtGridPoint(gridPt, pos, BaseBike.length, turnRadius); // see if this frame's motion changes the status

    //     // 
    //     // Do frame motion
    //     //
    //     if (_curTurn != TurnDir.kStraight)
    //     {
    //         // TODO: be consistent with degrees/radians
    //         // Do a turn iteration
    //         float dThetaRad = (_curTurn == TurnDir.kLeft ? -1f : 1f) * deltaT * BaseBike.speed / turnRadius;
    //         _turnTheta += dThetaRad * Mathf.Rad2Deg;
    //         float theta = _turnStartTheta + _turnTheta;
    //         pos = _turnAxis + new Vector3(turnRadius * Mathf.Sin(theta * Mathf.Deg2Rad), 0, turnRadius * Mathf.Cos(theta * Mathf.Deg2Rad));
    //         //Debug.Log(string.Format("startTheta: {0}", _turnStartTheta));             
    //         //Debug.Log(string.Format("turnTheta: {0}", _turnTheta)); 

    //         if (Mathf.Abs(_turnTheta) >= 90)
    //         {
    //             _curTurn = TurnDir.kStraight;
    //             angles.z = 0;
    //             angles.y = turnStartTheta[(int)heading] - 90f;
    //         }
    //         else
    //         {
    //             angles.z = -Mathf.Sin(_turnTheta * 2.0f * Mathf.Deg2Rad) * maxLean;
    //             angles.y = theta + (_curTurn == TurnDir.kLeft ? -1f : 1f) * 90f;
    //         }
    //         transform.eulerAngles = angles;

    //     }
    //     else
    //     {
    //         pos += GameTime.DeltaTime() * BaseBike.speed * GameConstants.UnitOffset3ForHeading(heading);
    //     }

    //     // Deal with this frame's motion

    //     bool atGridPt = AtGridPoint(gridPt, pos, BaseBike.length, turnRadius);

    //     if (atGridPt)
    //     {
    //         if (!prevAtGridPt)
    //         {
    //             // Just crossed onto a grid point
    //             // DealWithPlace(pos);

    //             // // Waiting to turn?
    //             // if (!prevAtGridPt && pendingTurn != TurnDir.kStraight && pendingTurn != TurnDir.kUnset && _curTurn == TurnDir.kStraight)
    //             // {
    //             //     // should be StartTurn()
    //             //     Heading prevHead = heading;
    //             //     _curTurn = pendingTurn;
    //             //     bb.TempSetHeading(GameConstants.NewHeadForTurn(heading, pendingTurn));

    //             //     // set up turn axis and angle
    //             //     _turnAxis = gridPt - (GameConstants.UnitOffset3ForHeading(prevHead) - GameConstants.UnitOffset3ForHeading(heading)) * turnRadius;
    //             //     _turnStartTheta = turnStartTheta[(int)prevHead] + (_curTurn == TurnDir.kRight ? 180f : 0f);
    //             //     _turnTheta = 0;
    //             //     //Debug.Log(string.Format("gridPt: {0}", gridPt));             
    //             //     //Debug.Log(string.Format("turnAxis: {0}", _turnAxis));                
    //             // }

    //             // bb.TempSetPendingTurn(TurnDir.kUnset);  // reset when you get to a grid point        
    //         }
    //     }

    //     transform.position = pos;

    // }

    public virtual void Update()
    {
        DecideToTurn();

        _curTurn = CurrentTurn();

        if (_curTurn == TurnDir.kStraight)
            DoStraight();
        else
            DoTurn();

    }

    protected TurnDir CurrentTurn()
    {
        // If we are not already in a turn, and a turn is pending
        // and we are close enough to the upcomng point,
        // Then set _curTurn, and _curTurnPt, and _curTurnCenter
        if (_curTurn != TurnDir.kStraight) // once set must be turned off by code
            return _curTurn;

        Vector2 nextGridPt =  BikeUtils.UpcomingGridPoint(bb.position, bb.heading);
        float nextDist = Vector2.Distance(bb.position, nextGridPt);

        if (  nextDist <= turnRadius 
            && pendingTurn != TurnDir.kStraight 
            && pendingTurn != TurnDir.kUnset)
            {
                _curTurnPt = nextGridPt;
                Heading nextHead = GameConstants.NewHeadForTurn(heading, pendingTurn);
                _curTurnCenter = nextGridPt - (GameConstants.UnitOffset2ForHeading(heading) - GameConstants.UnitOffset2ForHeading(nextHead)) * turnRadius; 
                _curTurnStartTheta = turnStartTheta[(int)heading] + (pendingTurn == TurnDir.kRight ? 180f : 0f);                               
                return pendingTurn;
            }

        return TurnDir.kStraight;
    }

    protected void DoStraight()
    {
        Vector3 pos = utils.Vec3(bb.position);
        Vector3 angles = transform.eulerAngles;
        angles.z = 0;
        angles.y = turnStartTheta[(int)heading] - 90f;
        transform.eulerAngles = angles;
        // Debug.Log(string.Format("Bike Pos: {0}", pos));        
        transform.position = pos;
    }

    protected void DoTurn()
    {
        // Do a turn iteration

        // What is the baseBike's fractional distance from the turn start to the turn end (total D => 2 * turnRadius)
        
        // Magnitude is the distance, if negative then we are heading towads it, positive is away
        float dotVal =  Vector2.Dot(GameConstants.UnitOffset2ForHeading(heading),  bb.position - _curTurnPt);

        // So, fractional  0 -> 1 distance along the turn is (dotVal + turnRadius) / (2* turnRadius)
        float frac = (dotVal + turnRadius) / (turnRadius * 2);

        if (frac < 1)    
        {
            // we're turning
            float thetaDeg = _curTurnStartTheta + 90f * frac * (_curTurn == TurnDir.kRight ? 1 : -1);
            //Debug.Log(string.Format("turnTheta: {0}", thetaDeg)); 

            // Position            
            Vector2 pos = _curTurnCenter + new Vector2(turnRadius * Mathf.Sin(thetaDeg * Mathf.Deg2Rad), turnRadius * Mathf.Cos(thetaDeg * Mathf.Deg2Rad));
            transform.position = utils.Vec3(pos, 0);

            // heading and lean
            Vector3 angles = transform.eulerAngles;            
            angles.z = -Mathf.Sin( frac * 180f * Mathf.Deg2Rad) * maxLean *  (_curTurn == TurnDir.kRight ? 1 : -1);
            angles.y = thetaDeg + (_curTurn == TurnDir.kLeft ? -1f : 1f) * 90f;  
            //Debug.Log(string.Format("lean: {0}", angles.z));            
            transform.eulerAngles = angles;          

        } else {
            // we're done
            //Debug.Log(string.Format("--------- turn done ---------- "));                  
            _curTurn = TurnDir.kStraight;
        }

    }

    public virtual void DecideToTurn() { }

    public void SetColor(Color newC)
    {
        transform.Find("Model/BikeMesh").GetComponent<Renderer>().material.color = newC;
        transform.Find("Trail").GetComponent<Renderer>().material.SetColor("_EmissionColor", newC);
    }

    public virtual void OnBikeAtPlace(Ground.Place place, bool justClaimed)
    {
        if (place == null)
        {
            Debug.Log("** FE bike got to null place");            
        } else {
            if (!justClaimed) 
            {
                ouchObj.SetActive(false); // restart in case the anim is already running
                ouchObj.SetActive(true);            
            }
        }
    }



    // Tools for AIs




    // AI Move stuff (here so player bike can display it)



  
}
