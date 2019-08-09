using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bike : MonoBehaviour
{
    public static readonly float length = 2.0f;
    public float speed =  15.0f;
    public float turnRadius = 1; // 2.0f;
    public float maxLean = 50.0f;

    protected GameObject ouchObj;

    protected static readonly Heading[][] newHeadForTurn = {
        // newHead = newHeadForTurn[oldHead][turnDir];
        new Heading[] { Heading.kNorth, Heading.kWest, Heading.kEast }, // N
        new Heading[] { Heading.kEast, Heading.kNorth, Heading.kSouth }, // E
        new Heading[] { Heading.kSouth, Heading.kEast, Heading.kWest }, // S
        new Heading[] { Heading.kWest, Heading.kSouth, Heading.kNorth } // W                 
    };

    protected static readonly float[] turnStartTheta = {
        90f, 180f, 270f, 0f
    };

    public static readonly float kRoot2Over2 = 2.0f / Mathf.Sqrt(2);

    public Heading heading = Heading.kNorth;
    public TurnDir _pendingTurn = TurnDir.kNone; // set and turn will start at next grid point
    
   // Heading _prevHead = Heading.kNorth; // if different than curHead we are in a turn
    Vector3 _turnAxis = Vector3.up; // just a dummy location
    float _turnStartTheta = 0f;
    float _turnTheta = 0f; // 0 is north, increases clockwise
    TurnDir _curTurn = TurnDir.kNone; 


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
    void Update()
    {
        Vector3 pos = transform.transform.position;
        Vector3 angles = transform.eulerAngles;
        float deltaT =  GameTime.DeltaTime();

        DecideToTurn();

        Vector3 gridPt = NearestGridPoint(pos, Ground.gridSize); 
        bool prevAtGridPt =  AtGridPoint(gridPt, pos, length, turnRadius); // see if this frame's motion changes the status

         // 
         // Do frame motion
         //
        if (_curTurn != TurnDir.kNone)
        {
            // TODO: be consistent with degrees/radians
            // Do a turn iteration
            float dThetaRad =  (_curTurn == TurnDir.kLeft ? -1f : 1f) * deltaT * speed / turnRadius;
            _turnTheta +=  dThetaRad * Mathf.Rad2Deg;
            float theta = _turnStartTheta + _turnTheta;
            pos = _turnAxis + new Vector3( turnRadius * Mathf.Sin(theta * Mathf.Deg2Rad ), 0,  turnRadius * Mathf.Cos(theta * Mathf.Deg2Rad ));
            //Debug.Log(string.Format("startTheta: {0}", _turnStartTheta));             
            //Debug.Log(string.Format("turnTheta: {0}", _turnTheta)); 

            if ( Mathf.Abs(_turnTheta) >= 90 ) {
                _curTurn = TurnDir.kNone;
                angles.z = 0;
                angles.y = turnStartTheta[(int)heading] - 90f;
            } else {
                angles.z =  -Mathf.Sin(_turnTheta * 2.0f * Mathf.Deg2Rad) * maxLean;        
                angles.y =  theta + (_curTurn == TurnDir.kLeft ? -1f : 1f) *  90f;
            }
            transform.eulerAngles = angles;            

        } else {
            pos += GameTime.DeltaTime() * speed * GameConstants.unitOffsetForHeading[(int)heading];
        }

        // Deal with this frame's motion
 
        bool atGridPt =  AtGridPoint(gridPt, pos, length, turnRadius); 
        // Just crossed onto a grid pint?  
        if (atGridPt && !prevAtGridPt)
        {
            DealWithPlace(pos);

            // Waiting to turn?
            if ( !prevAtGridPt && _pendingTurn != TurnDir.kNone && _curTurn == TurnDir.kNone)
            {
                // should be StartTurn()
                Heading prevHead = heading;
                _curTurn = _pendingTurn;
                heading = newHeadForTurn[(int)heading][(int)_pendingTurn];
                _pendingTurn = TurnDir.kNone;

                // set up turn axis and angle
                _turnAxis = gridPt - (GameConstants.unitOffsetForHeading[(int)prevHead] - GameConstants.unitOffsetForHeading[(int)heading]) * turnRadius;
                _turnStartTheta = turnStartTheta[(int)prevHead] + (_curTurn == TurnDir.kRight ? 180f : 0f);
                _turnTheta = 0;
                //Debug.Log(string.Format("gridPt: {0}", gridPt));             
                //Debug.Log(string.Format("turnAxis: {0}", _turnAxis));                
            }
        }

        transform.position = pos;
      
    }


    public virtual void DecideToTurn() {}


    public void SetColor(Color newC)
    {
        transform.Find("BikeMesh").GetComponent<Renderer>().material.color = newC;
        transform.Find("Trail").GetComponent<Renderer>().material.SetColor("_EmissionColor", newC);
    }

    protected virtual Ground.Place DealWithPlace(Vector3 pos) // returns place in case override wants to do something with it.
    {
        Ground g = GameMain.GetInstance().ground;
        Ground.Place p = g.GetPlace(pos);
        if (p == null) {
            p = g.ClaimPlace(this, pos);
        } else {
            ouchObj.SetActive(false); // restart in case the anim is already running
            ouchObj.SetActive(true);
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
        return new Vector3( Mathf.Round(pos.x * invGridSize) * gridSize, pos.y, Mathf.Round(pos.z * invGridSize) * gridSize);
    }

}
