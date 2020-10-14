using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamGameCode;
using BikeControl;

public class AutoMat<T>
{
    protected Dictionary<T,Material> MaterialDict;

    public AutoMat()
    {
        MaterialDict = new Dictionary<T,Material>();
    }

    // expected usage:
    //

    public Material GetMaterial(T key)
    {
        return MaterialDict.ContainsKey(key) ? MaterialDict[key] : null;
    }

    public Material AddMaterial(T key, Material mat)
    {
        if (MaterialDict.ContainsKey(key))
            return null;

        MaterialDict[key] = mat;
        return mat;
    }
}

public abstract class FrontendBike : MonoBehaviour
{
    public IBike bb = null;
    protected IBeamAppCore appCore = null;
    protected IBikeControl control = null;
    protected FeGround feGround;

    protected FeBikeLabel bikeLabel;

    protected static AutoMat<Color> autoMat;

    // Stuff that really lives in backend.
    // TODO: maybe get rid of this? Or maybe it's ok
    public Heading heading { get => bb.baseHeading; }

    // Temp (probably) stuff for refactoring to add BaseBike
    public TurnDir pendingTurn { get => bb.basePendingTurn; }

    public float turnRadius = 1.5f;

    public float maxLean = 80.0f; // 40 is real
    public bool isLocal;

    public Vector2 curPos2d;

    protected GameObject ouchObj;
    protected AudioSource engineSound;

    protected static readonly float[] turnStartTheta = {
        90f, 180f, 270f, 0f
    };

    protected TurnDir _curTurn = TurnDir.kStraight;
    protected Vector2 _curTurnPt; // only has meaning when curTuen is set
    protected Vector2 _curTurnCenter; // only has meaning when curTuen is set
    protected float _curTurnStartTheta;
    private long _prevGameTime = 0;

    protected BeamPlace prevPlaceVisited;

    protected abstract void CreateControl();

    public virtual void Awake()
    {
        if (autoMat == null)
            autoMat = new AutoMat<Color>();

        isLocal = true; // default
        ouchObj = transform.Find("Ouch").gameObject;
        engineSound = transform.Find("EngineSound").gameObject.GetComponent<AudioSource>();
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
        engineSound.mute = false;
        engineSound.volume = 1.0f;
        engineSound.pitch = UnityEngine.Random.Range(.9f, 1.01f); // Make 'em chorus a little
    }

    // Important: Setup() is not called until after Awake() and Start() have been called on the
    // GameObject and components. Both of those are called when the GO is instantiated
    public virtual void Setup(IBike beBike, FeGround fGround, IBeamAppCore core)
    {
        appCore = core;
        bb = beBike;
        feGround = fGround;
        transform.position = utils.Vec3(bb.basePosition); // Is probably already set to this
        SetColor(utils.ColorFromName(bb.team.Color));
        CreateControl();
        control.Setup(beBike, core);
        _prevGameTime = core.CurrentRunningGameTime;

        bikeLabel = transform.Find("BikeLabel").GetComponent<FeBikeLabel>();
        bikeLabel.Setup(this);
        ShowLabel(false);
    }

    public virtual void Update()
    {
        long curGameTime = appCore.CurrentRunningGameTime;
        int frameMs = (int)(curGameTime - _prevGameTime);
        _prevGameTime = curGameTime;

        if (frameMs == 0)
            return;

        curPos2d = bb.DynamicState(curGameTime).position;
        control.Loop(curGameTime, frameMs); // TODO: is this the right place to get the frameTime?
        _curTurn = CurrentTurn();

        if (_curTurn == TurnDir.kStraight)
            DoStraight();
        else
            DoTurn();

        bikeLabel.UpdatePos();

    }

    protected TurnDir CurrentTurn()
    {
        // If we are not already in a turn, and a turn is pending
        // and we are close enough to the upcomng point,
        // Then set _curTurn, and _curTurnPt, and _curTurnCenter
        if (_curTurn != TurnDir.kStraight) // once set must be turned off by code
            return _curTurn;

        Vector2 nextGridPt =  BikeUtils.UpcomingGridPoint(curPos2d, bb.baseHeading);
        float nextDist = Vector2.Distance(curPos2d, nextGridPt);

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
        Vector3 pos = utils.Vec3(curPos2d);
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
        float dotVal =  Vector2.Dot(GameConstants.UnitOffset2ForHeading(heading),  curPos2d - _curTurnPt);

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
        Renderer r = transform.Find("Model/BikeMesh").GetComponent<Renderer>();
        Material mat = autoMat.GetMaterial(newC);
        if (mat == null)
        {
            mat = r.material;
            //mat = new Material(r.material);
            mat.color = newC;
            autoMat.AddMaterial(newC, mat);
        }
        r.sharedMaterial = autoMat.GetMaterial(newC);

        Color smokeC = ( newC + Color.white) / 2.0f; // halfway to white (reduce saturation + brighten)
        ParticleSystem ps = transform.Find("Smoke").GetComponent<ParticleSystem>();
        ParticleSystem.ColorOverLifetimeModule  colorModule = ps.colorOverLifetime;
        Gradient grad  = new Gradient();
        grad.SetKeys(
             new GradientColorKey[] { new GradientColorKey(smokeC, 1.0f) },
             new GradientAlphaKey[] { new GradientAlphaKey(0.7f, 0.0f), new GradientAlphaKey(.5f, .5f),  new GradientAlphaKey(0, 1.0f) }
        );
        colorModule.color = grad;

    }

    public virtual void OnPlaceClaimed(BeamPlace place)
    {
        if (place == prevPlaceVisited)
        {
            // TODO: this is the same place we just hit/claimed. Figure out what's up (time backup?)
            // Or at least log a message
        } else {
            if ( prevPlaceVisited?.bike?.bikeId == bb.bikeId) // might be null, might not have a bike
            {
                // BUG: This won;t work on mass restore. Places would need a per-bike seq #
                feGround.SetupConnector(prevPlaceVisited, place);
            }
            prevPlaceVisited = place;
        }
    }

    public virtual void OnPlaceHit(BeamPlace place)
    {
        ouchObj?.SetActive(false); // restart in case the anim is already running
        ouchObj?.SetActive(true);
        prevPlaceVisited = place;
    }

    public void EnableSound(bool doIt)
    {
        //engineSound.mute = !doIt;
    }

    public void EngineVolume(float vol)
    {
        engineSound.volume = vol;
    }

    public void ShowLabel(bool doIt)
    {
        bikeLabel.gameObject.SetActive(doIt);
    }

    // Tools for AIs




    // AI Move stuff (here so player bike can display it)




}
