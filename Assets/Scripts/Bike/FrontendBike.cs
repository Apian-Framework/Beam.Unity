using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;
using BikeControl;

public abstract class FrontendBike : MonoBehaviour
{
    public IBike bb = null;
    protected IBeamAppCore appCore = null;

    protected IBikeControl control = null;

    // Stuff that really lives in backend.
    // TODO: maybe get rid of this? Or maybe it's ok
    public Heading heading { get => bb.heading; }

    // Temp (probably) stuff for refactoring to add BaseBike
    public TurnDir pendingTurn { get => bb.pendingTurn; }

    public float turnRadius = 1.5f;

    public float maxLean = 40.0f;

    public bool isLocal;

    protected GameObject ouchObj;

    protected static readonly float[] turnStartTheta = {
        90f, 180f, 270f, 0f
    };

    protected TurnDir _curTurn = TurnDir.kStraight;
    protected Vector2 _curTurnPt; // only has meaning when curTuen is set
    protected Vector2 _curTurnCenter; // only has meaning when curTuen is set
    protected float _curTurnStartTheta;

    protected abstract void CreateControl();

    public virtual void Awake()
    {
        isLocal = true; // default
        ouchObj = transform.Find("Ouch").gameObject;
    }

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Initialize pointing direction based on heading;
        // assumes pos and heading are set on creation
        Vector3 angles = transform.eulerAngles;
        angles.z = 0;
        angles.y = turnStartTheta[(int)heading] - 90f;
        transform.eulerAngles = angles;
        ouchObj.SetActive(false);
    }

    // Important: Setup() is not called until after Awake() and Start() have been called on the
    // GameObject and components. Both of those are called when the GO is instantiated
    public virtual void Setup(IBike beBike, IBeamAppCore core)
    {
        appCore = core;
        bb = beBike;
        transform.position = utils.Vec3(bb.position); // Is probably already set to this
        SetColor(utils.hexToColor(bb.team.Color));
        CreateControl();
        control.Setup(beBike, core);
    }

    public virtual void Update()
    {
        control.Loop(Time.deltaTime); // TODO: is this the right place to get the frameTime?

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

    public void SetColor(Color newC)
    {
        transform.Find("Model/BikeMesh").GetComponent<Renderer>().material.color = newC;
        transform.Find("Trail").GetComponent<Renderer>().material.SetColor("_EmissionColor", newC);
    }

    public virtual void OnPlaceHit(BeamPlace place)
    {
        ouchObj?.SetActive(false); // restart in case the anim is already running
        ouchObj?.SetActive(true);
    }



    // Tools for AIs




    // AI Move stuff (here so player bike can display it)




}
