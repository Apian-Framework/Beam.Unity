using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameCamera : MonoBehaviour {

    public class LookParams
    {
        public float AngleRad {get; set;}
        public float DecayRate {get; set;}
    }

	public event EventHandler<LookParams> LookAroundEvt; 

	protected const float kZeroDist = .001f;		
    protected const float kMaxRotRate = 180f;
    
    public CameraMode _curMode = null;
    public CamModeID  _curModeID = CamModeID.kNormal;
	
	protected Camera _thisCamera;
    public BeamMain feMain;
 
    //
    // Basic util to move from one place to another
    //

    protected Vector3 _curVel;  // used by damped motion code
    protected float _moveStartTime;
    protected float _moveStartFOV;        
    protected bool _bMoving = false;
    protected bool _bRotating = false;
    protected bool _bZooming = false;    

	public CamModeID getMode() => _curModeID;

	void Start ()  // Monobehavior start
    {
        feMain = BeamMain.GetInstance();
		_thisCamera = (Camera)GetComponent("Camera");
        ModeNormal mode = (ModeNormal)SetMode(CamModeID.kNormal);
        mode.init(this);            
	}

	void Update () 
	{
        if (_curMode != null)
            _curMode.update();	
	}
	

    public void StartMotion()
    { 
        _curVel = Vector3.zero;
        
        _bMoving = true;
        _bRotating = true;
        _bZooming = true; 
        
        _moveStartTime = Time.time;
        _moveStartFOV = _thisCamera.fieldOfView;    
         
    }      
    
    // return true when done
    public bool MoveTowards(Vector3 targetPos, Vector3 targetLookAt, float targetFOV, float targetMoveSecs, float closeEnough)
    {
        Transform thisT = _thisCamera.transform;
        targetMoveSecs *= GameTime.GetRate();
        
        if (_bMoving)
        {                            
            thisT.position = Vector3.SmoothDamp(thisT.position, targetPos, ref _curVel, targetMoveSecs);
            if (Vector3.Distance(thisT.position, targetPos) < closeEnough)
            {          
                _bMoving = false;
                //Debug.Log("Not moving!");                         
            }           
        }
         
        if (_bRotating)
        {
            Vector3 relativePos = targetLookAt - thisT.position;
            Quaternion targetRotation = Quaternion.LookRotation(relativePos); 
            var step = kMaxRotRate * GameTime.DeltaTime();
            thisT.rotation = Quaternion.RotateTowards(thisT.rotation, targetRotation, step);            
            // gotten there? Note that the rotation can't be considered done until the motion is done
            // Since the latter affects the former
            if  ((_bMoving == false) && (Quaternion.Angle(thisT.rotation, targetRotation) < 5))
            {
                _bRotating = false; 
            }       
        }
         
        if (_bZooming)
        {
            if (targetFOV <= 0)
            {
                _bZooming = false;
            }
            else
            {
                float dt = (Time.time - _moveStartTime) / (targetMoveSecs*2);
                float ang = Mathf.Lerp(_moveStartFOV, targetFOV, dt);            
                _thisCamera.fieldOfView = ang;
             
                // gotten there?
                if  (dt >= 1.0)
                {
                    _bZooming = false;
                }
            }
        }    
        
        if (!(_bMoving || _bRotating || _bZooming))
            Debug.Log("Got there!");
        
        return !(_bMoving || _bRotating || _bZooming); // true if "we are there"
    }       

    // You need to call this followed by the init() for your selected mode
    protected CameraMode SetMode(CamModeID modeID)
    {
        CameraMode newMode = null;
     
        switch (modeID)
        {
        case CamModeID.kNormal:
            newMode = new ModeNormal();
            break;
            
        case CamModeID.kMoveToPos:
            newMode = new ModeMovingToPos();
            break;       

        case CamModeID.kMoveToTarget:
            newMode = new ModeMovingToTarget();
            break;     
            
        case CamModeID.kOrbit:
            newMode = new ModeOrbit();
            break;     

        case CamModeID.kBikeView:
            newMode = new ModeBikeView();
            break;                        
   
        case CamModeID.kOverheadView:
            newMode = new ModeOverheadView();
            break;                        

        case CamModeID.kEnemyView:
            newMode = new ModeEnemyView();
            break; 
        
        }
     
        if (newMode != null)    
        {
             if (_curMode != null)
                _curMode.end();               
            _curMode = newMode;      
            _curModeID = modeID;
        }   
        
        return newMode;
    }
    
	
	public void MoveCameraToPos(Vector3 newPos, Vector3 newLookAt, float newFOV, float moveSecs)
	{
        ModeMovingToPos mode = (ModeMovingToPos)SetMode(CamModeID.kMoveToPos);
        mode.init(this, newPos, newLookAt, newFOV, moveSecs);
	}	
    
	public void MoveCameraToTarget(GameObject target, float finalDist , float finalHeight, float moveSecs, float closeEnough = kZeroDist)
	{
        ModeMovingToTarget mode = (ModeMovingToTarget)SetMode(CamModeID.kMoveToTarget);
        mode.init(this, target, finalDist, finalHeight, moveSecs, closeEnough);
	}	

    // Maintain current distance from object and current height. >0 is CCW. Look at target's pos plus offset
    public void StartOrbit(GameObject target, float degPerSec, Vector3 offset)
    {
        ModeOrbit mode = (ModeOrbit)SetMode(CamModeID.kOrbit);
        mode.init(this, target, degPerSec, offset);
    }       

    public void StartBikeMode(GameObject targetBike)
    {
        ModeBikeView mode = (ModeBikeView)SetMode(CamModeID.kBikeView);       
        mode.init(this, targetBike);        
    }    

    public void StartEnemyView(GameObject playerBike)
    {
        ModeEnemyView mode = (ModeEnemyView)SetMode(CamModeID.kEnemyView);       
        mode.init(this, playerBike);        
    } 

    public void StartOverheadMode(GameObject targetBike)
    {
        ModeOverheadView mode = (ModeOverheadView)SetMode(CamModeID.kOverheadView);
        mode.init(this, targetBike);        
    }    

    public void LookAround(float angle, float decayRate)
    {
        LookAroundEvt?.Invoke(this, new LookParams(){AngleRad=angle, DecayRate=decayRate});
    }
    

    //
    // Camera modes (TODO: move out of here)
    // Another TODO: this whole thing is awkward and weird.
    //
    
    public class CameraMode
    {
        public GameCamera _theGameCam;   

        public virtual void init(GameCamera cam)
        {
            _theGameCam = cam;
        }
 
        public virtual void update() {} 
        public virtual void end() {}                

        // For moving a camera towards a target object
        protected Vector3 TargetCamPos(GameObject target, Vector3 posOffset)
        {       
            return target.transform.TransformPoint(posOffset);
        }
                
        protected Vector3 TargetCamLookat(GameObject target, float lookAngleRad, float lookHeight)
        {     
            Vector3 offset = new Vector3(-100 * Mathf.Sin(lookAngleRad), lookHeight, 100 *  Mathf.Cos(lookAngleRad));
            return target.transform.TransformPoint(offset);
        }          

        // Some Utils
        protected List<GameObject> ClosestBikes(GameObject thisBike, int maxCount)
        {    
            if (thisBike == null)
                return new List<GameObject>();
            List<GameObject> feBikes = _theGameCam.feMain.frontend.GetBikeList();
            return feBikes.Where(b => b != thisBike)
                .OrderBy(b => Vector3.Distance(b.transform.position, thisBike.transform.position)).Take(maxCount) // GameObjects
                .ToList();        
        }


    }

    
    public enum CamModeID
    {
        kNormal = 0,
        kMoveToPos,
        kMoveToTarget,
        kOrbit,
        kBikeView,   
        kEnemyView,
        kOverheadView,     
    };
    
    //
    public class ModeNormal : CameraMode
    {        
        // Do nothing
    }
    
    public class ModeMovingToPos : CameraMode
    {
        // Move from current place to another fixed place with given view
        protected Vector3 _targetPos;
        protected Vector3 _targetLookAt;
        protected float   _targetFOV;
        protected float   _targetMoveSecs = 1.0f;

        public virtual void init(GameCamera cam, Vector3 pos, Vector3 lookPos, float fov, float moveSecs)
        {
            base.init(cam);  

            _targetPos = pos;
            _targetLookAt = lookPos;
            _targetFOV = fov;
            _targetMoveSecs = moveSecs;
            
            cam.StartMotion();
        }      
        
        public override void update()
        {
            if (_theGameCam.MoveTowards(_targetPos, _targetLookAt, _targetFOV, _targetMoveSecs, kZeroDist) )
                _theGameCam.SetMode(CamModeID.kNormal).init(_theGameCam);            
        }   
    }
    
    public class ModeMovingToTarget : CameraMode
    {
        // Move from current place to another fixed place with given view
        protected GameObject _target;
        protected float   _targetMoveSecs = 1.0f;

        protected float _finalDist = 0; 
        protected float _finalHeight = 0;

        protected float _closeEnough;
        
        public virtual void init(GameCamera cam, GameObject target, float finalDist, float finalHeight, float moveSecs, float closeEnough)
        {
            base.init(cam);  

            _target = target;
            _finalDist = finalDist;
            _finalHeight = finalHeight;
            _targetMoveSecs = moveSecs;
            _closeEnough = closeEnough;   
            cam.StartMotion();
        }      
        
        public override void update()
        {
            // TODO: handle "target is gone" in a more generic way
            if (_target == null) // target went away (gameobject refs get autonulled on destroy)
            {
                _theGameCam.SetMode(CamModeID.kNormal).init(_theGameCam);  // when we get there switch to "normal"
            } else {            
                // Our target is on the line between the camera and the target, "dist" away from the target and at a height of "finalHeight".
                Vector3 posOffset =  _finalDist * (_theGameCam.transform.position -_target.transform.position).normalized;
                posOffset.y = _finalHeight;
            
                Vector3 pos = TargetCamPos(_target, posOffset);
                    
                if (_theGameCam.MoveTowards(pos, _target.transform.position, -1, _targetMoveSecs, _closeEnough))
                    _theGameCam.SetMode(CamModeID.kNormal).init(_theGameCam);  // when we get there switch to "normal"
            }
                
        }   
    }

    public class ModeOrbit : CameraMode
    {    
        // Orbit around a gameObject as it moves
        protected GameObject _targetObj;
        protected float _degPerSec;
        
        protected float _radius;
        protected float _curAngle;
        protected float _height;
        
        protected Vector3 _offset;
        
        public virtual void init(GameCamera cam, GameObject target, float degPerSec, Vector3 offset)
        {
            base.init(cam);   
            
            _targetObj = target;
            _degPerSec = degPerSec;
            _offset = offset;
            
            Vector3 targetPos = _targetObj.transform.position;
            targetPos += offset;

            // From target (w/offset) to camera
            Vector3 toCam =  _theGameCam.transform.position - targetPos;
            
            _height = toCam.y;
            _radius = toCam.magnitude;   
            _curAngle = Mathf.Atan2(toCam.z, toCam.x);
        }
        
        public override void update()
        {
            _curAngle += GameTime.DeltaTime() * _degPerSec * Mathf.Deg2Rad;
            
            Vector3 pos = new Vector3(_radius * Mathf.Cos(_curAngle), _height, _radius * Mathf.Sin(_curAngle));
            pos +=  _offset;
            pos += _targetObj.transform.position;

            _theGameCam.transform.position = pos;
            _theGameCam.transform.LookAt(_targetObj.transform.position + _offset);
        }
    }
    
    public class ModeOverheadView : CameraMode
    {
        protected GameObject _bike;
        protected float _height;
        protected float _viewHeight;

        public virtual void init(GameCamera cam, GameObject bike)
        {
            base.init(cam);   

            _bike = bike;
            _height = 60.0f;
            _viewHeight = -1000f;
            _theGameCam.StartMotion();
        }

        public override void update()
        {
            if (_bike == null) // target went away (gameobject refs get autonulled on destroy)
            {
                _theGameCam.SetMode(CamModeID.kNormal).init(_theGameCam);  // when we get there switch to "normal"
            } else {
                Vector3 posOffset = new Vector3(0, _height, 0);            
                Vector3 pos = TargetCamPos(_bike, posOffset);
                Vector3 lookAt = TargetCamLookat(_bike, 0, _viewHeight);                     
                _theGameCam.MoveTowards(pos, lookAt, -1, .2f, kZeroDist);  
            }

        }
                
    }

    public class ModeEnemyView : CameraMode
    {  
       protected GameObject thisBike;
       protected GameObject enemyBike;

        protected float camHeight = 3.0f;
        protected float radius = 4.0f;
        protected int maxEnemies = 6;
       public virtual void init(GameCamera cam, GameObject bike)
        {
            base.init(cam);   
            thisBike = bike; 
            enemyBike = GetClosestEnemy();
            _theGameCam.LookAroundEvt += OnLookAround;            
            _theGameCam.StartMotion();
        }       

        protected GameObject GetClosestEnemy() 
        {
            return ClosestBikes(thisBike, 1).FirstOrDefault();
        } 

        protected GameObject GetNextEnemy() 
        {
            List<GameObject> enemies = ClosestBikes(thisBike, maxEnemies);
            int curIdx = enemies.IndexOf(enemyBike);
            if (curIdx < 0)
                return GetClosestEnemy();
            return enemies[(curIdx+1)%enemies.Count];
        }

        protected GameObject GetPrevEnemy() 
        {
            List<GameObject> enemies = ClosestBikes(thisBike, maxEnemies);
            int curIdx = enemies.IndexOf(enemyBike);
            if (curIdx < 0)
                return GetClosestEnemy();
            return enemies[(curIdx+enemies.Count-1)%enemies.Count];
        }
        public override void update()
        {
            if (thisBike == null) // target went away (gameobject refs get autonulled on destroy)
            {
                _theGameCam.SetMode(CamModeID.kNormal).init(_theGameCam);  // when we get there switch to "normal"
            } else {
                if (enemyBike == null)
                {
                    Vector3 posOffset = new Vector3(0, camHeight, -radius);            
                    Vector3 pos = TargetCamPos(thisBike, posOffset);
                    Vector3 lookAt = TargetCamLookat(thisBike, 0, 0);          
                    _theGameCam.MoveTowards(pos, lookAt, -1, .2f, kZeroDist);  
                } else {
                    Vector3 lookVec = enemyBike.transform.position - thisBike.transform.position;
                    Vector3 posOffset = lookVec.normalized * -radius;
                    posOffset.y = camHeight;
                     _theGameCam.MoveTowards(thisBike.transform.position + posOffset, enemyBike.transform.position, -1, .2f, 0);
                }
            }
        }

        protected void OnLookAround(object sender, LookParams lp)        
        {
            if (Math.Sign(lp.AngleRad) < 0 ) // ngative is "next)
                enemyBike = GetNextEnemy();
            else
                enemyBike = GetPrevEnemy();
        }          

    }

    
    // set up behind a player and look the same direction - with a rate limit
    public class ModeBikeView : CameraMode
    {             
        protected float _highHeight;
        protected float _lowHeight;

        protected GameObject _bike;
        protected float _maxDegPerSec;
        
        protected float _radius;
        protected float _curAngle;
        protected float _height;
        protected float _viewHeight;
        
        protected float _lookAngle;
        protected float _lookDecayRate;

        protected bool _inPlace; // initially false until cam is behind player
        

        public virtual void init(GameCamera cam, GameObject bike)
        {
            base.init(cam);   

            _theGameCam.LookAroundEvt += OnLookAround;
           
            _bike = bike;
            _maxDegPerSec = 120;
            _radius = 3.0f;
            _height = 3.0f;
            _viewHeight = 0f;
            _highHeight = 60f;
            _lowHeight = 3.0f;
            _lookAngle = 0f;
            _lookDecayRate = .5f; // deg/sec
            _theGameCam.StartMotion();
        }
          

        public override void update()
        {
            if (_bike == null) // target went away (gameobject refs get autonulled on destroy)
            {
                _theGameCam.SetMode(CamModeID.kNormal).init(_theGameCam);  // when we get there switch to "normal"
            } else {
                Vector3 posOffset = new Vector3(0, _height, -_radius);            
                Vector3 pos = TargetCamPos(_bike, posOffset);
                Vector3 lookAt = TargetCamLookat(_bike, _lookAngle, _viewHeight);                     
                _theGameCam.MoveTowards(pos, lookAt, -1, .2f, kZeroDist);  

                float lookSign = Mathf.Sign(_lookAngle);
                float absLA = Mathf.Max(0, Math.Abs(_lookAngle) - (GameTime.DeltaTime() * _lookDecayRate));
                _lookAngle = lookSign * absLA;
            }

        }

        protected void OnLookAround(object sender, LookParams lp)        
        {
            if (Math.Sign(lp.AngleRad) == -Math.Sign(_lookAngle)) // NOTE: *NOT* Mathf.Sign() which is incorrect for 0
            {               
                _lookAngle = 0; // hitting left while looking right immediates centers view
            } else {
                _lookAngle += lp.AngleRad;
                _lookDecayRate = lp.DecayRate;
            }
        }        
    }
        
}
