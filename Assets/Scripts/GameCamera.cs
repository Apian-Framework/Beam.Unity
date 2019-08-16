using UnityEngine;
using System;
using System.Collections.Generic;

public class GameCamera : MonoBehaviour {
	
	protected const float kZeroDist = .001f;		
	//protected const float kMaxRotRate = 120f;
    protected const float kMaxRotRate = 180f;
    
    public CameraMode _curMode = null;
    public CamModeID  _curModeID = CamModeID.kNormal;
	
	protected Camera _thisCamera;
 
    //
    // Basic util to move from one place to another
    //

    protected Vector3 _curVel;  // used by damped motion code
    
    protected float _moveStartTime;
    protected float _moveStartFOV;        
    
    protected bool _bMoving = false;
    protected bool _bRotating = false;
    protected bool _bZooming = false;    

        
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
            
            // constrain to field + endzone
            // float max = FBField.kWidth * .5f + 2;
            // targetPos.x = Mathf.Clamp(targetPos.x, -max, max);
            // max = FBField.kLength * .5f + FBField.kEndZoneDepth;
            // targetPos.z = Mathf.Clamp(targetPos.z, -max, max);               
            
            thisT.position = Vector3.SmoothDamp(thisT.position, targetPos, ref _curVel, targetMoveSecs);
     
            // Debug.Log(string.Format("Current dist: {0}, Current Vel: {1}",Vector3.Distance(thisT.position, targetPos),_curVel));

            if (Vector3.Distance(thisT.position, targetPos) < closeEnough)
            {          
                _bMoving = false;
                Debug.Log("Not moving!");                         
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
    
    
    
    
    public class CameraMode
    {
        public GameCamera _theGameCam; 
     
        protected Dictionary<int,dynamic> _cmdDispatch = new Dictionary<int, dynamic>();        

        public virtual void init(GameCamera cam)
        {
            _theGameCam = cam;
        }
 
        public virtual void update() {} 
        public virtual void end() {}         
        public virtual void handleCmd(int cmd, object param) {}           

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
    }
    
    
    // States
    public enum CamModeID
    {
        kNormal = 0,
        kMoveToPos,
        kMoveToTarget,
        kOrbit,
        kBikeView,        
        // kSweep,
        // kBallChase
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
            // Our target is on the line between the camera and the target, "dist" away from the target and at a height of "finalHeight".
            Vector3 posOffset =  _finalDist * (_theGameCam.transform.position -_target.transform.position).normalized;
            posOffset.y = _finalHeight;
          
            Vector3 pos = TargetCamPos(_target, posOffset);
                   
            if (_theGameCam.MoveTowards(pos, _target.transform.position, -1, _targetMoveSecs, _closeEnough))
                _theGameCam.SetMode(CamModeID.kNormal).init(_theGameCam);  // when we get there switch to "normal"
                
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

           // Debug.Log(string.Format("{0}",pos));
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
        
        public enum Commands
        {
            kInit = 0,
            kLookAround = 1,
            kToggleHighLow = 2,
            kCount = 3
        }

        public class LookParams
        {
            public float angle;
            public float decayRate;
            public LookParams(float a, float d)  { angle = a; decayRate = d; }
        }

        public virtual void init(GameCamera cam, GameObject bike)
        {
            base.init(cam);   
            _cmdDispatch[(int)Commands.kInit] = new Action<object>( (o) => {} );  // TODO: &&&& First command invoke causes a delay "blip".  This is a bad answer.
            _cmdDispatch[(int)Commands.kLookAround] = new Action<object>(o => lookAround(o));
            _cmdDispatch[(int)Commands.kToggleHighLow] = new Action<object>(o => ToggleHeight(o));            

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
            _cmdDispatch[(int)Commands.kInit](null);
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


        protected void lookAround( object param)
        {
            LookParams lp = (LookParams)param;
            if (Math.Sign(lp.angle) == -Math.Sign(_lookAngle)) // NOTE: *NOT* Mathf.Sign() which is incorrect for 0
            {               
                _lookAngle = 0; // hitting left while looking right immediates centers view
            } else {
                _lookAngle += lp.angle;
                _lookDecayRate = lp.decayRate;
            }
        }

        protected void ToggleHeight(object param)
        {
            _height = (_height == _lowHeight) ? _highHeight : _lowHeight;
            _viewHeight = (_height == _lowHeight) ? 0 : -1000f; // &&&&& TODO: constants!
        }

        public override void handleCmd(int cmd, object param)
        {
            _cmdDispatch[cmd](param);            
        }
        
    }
        
    // public class ModeSweep : ModeOrbit
    // {       

    //     float _leftAng;
    //     float _rightAng;        

        
    //     public virtual void init(GameCamera cam, GameObject target, float degPerSec, Vector3 offset, float leftDeg, float rightDeg)
    //     {
    //         base.init(cam, target, degPerSec, offset);    
    
    //         _leftAng = leftDeg * Mathf.Deg2Rad;
    //         _rightAng = rightDeg * Mathf.Deg2Rad;
            
    //     }
        
    //     public override void update()
    //     {
    //         base.update();
            
    //         if (_degPerSec > 0) // CCW
    //         {
    //             // &&& THESE ARE WRONG! Mathf.DeltaAngle takes degrees (really!)
    //             // ...but it kinda works since it;s just comparing to 0 so I'll just leave it alone
    //             if ( Mathf.DeltaAngle(_curAngle, _rightAng) < 0)
    //                 _degPerSec = -_degPerSec;      
    //         }
    //         else // CW
    //         {
    //             if ( Mathf.DeltaAngle(_curAngle, _leftAng) > 0)
    //                 _degPerSec = -_degPerSec;                    
    //         }
 
    //     }
    // }        
   
   
    // chase a flying ball
    // public class ModeBallChase : CameraMode
    // {       
    //     protected FBBall _ball;
    //     protected float _maxDegPerSec;
        
    //     protected float _radius;
    //     protected float _curAngle;
    //     protected float _height;
        
    //     protected bool _inPlace; // initially false until cam is behind player
        
    //     public virtual void init(GameCamera cam, FBBall ball)
    //     {
    //         base.init(cam);   
            
    //         _ball = ball;
    //         _maxDegPerSec = 30;
    //         _radius = 5.0f;
    //         _height = 3.0f;
            
    //         _theGameCam.StartMotion();
            
    //     }
        
    //     protected Vector3 TargetCamPos()
    //     {    
    //         Vector3 offset = new Vector3(0, 0, -_radius);  
            
    //         Vector3 pos = _ball.transform.TransformPoint(offset);
            
    //         if (pos.y < 2f)
    //             pos.y = 2f;
            
    //         return pos;
    //     }
        
    //     protected Vector3 TargetCamLookat()
    //     {    
    //         //Vector3 offset = new Vector3(0, 0, 100);    
    //         //return _ball.transform.TransformPoint(offset);
            
    //         return _ball.transform.position;
    //     }         

    //     public override void update()
    //     {
    //         Vector3 pos = TargetCamPos();
    //         Vector3 lookAt = TargetCamLookat();
            
    //         _theGameCam.MoveTowards(pos, lookAt, -1, .25f);       
    //     }
        
    // }
             
   
    //
    // GameCamera Code
    //

	// Use this for initialization
	void Start () {
		
		_thisCamera = (Camera)GetComponent("Camera");
        ModeNormal mode = (ModeNormal)SetMode(CamModeID.kNormal);
        mode.init(this);            
	
	}
	
    // You need to call this followed by the init() for your selected mode
    // Not sure it should really be public
    public CameraMode SetMode(CamModeID modeID)
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
   
        // case CamModeID.kSweep:
        //  newMode = new ModeSweep();
        //  break;                              

        // case CamModeID.kBallChase:
        //  newMode = new ModeBallChase();
        //  break;             
            
        }
     
        if (newMode != null)    
        {
             if (_curMode != null)
                _curMode.end();     
            _curMode = null;            
            _curMode = newMode;      
            _curModeID = modeID;
        }   
        
        return newMode;
    }
    
	void Update () 
	{
        if (_curMode != null)
            _curMode.update();	
	}
	
	// Call this to set the camera in motion	
	public void MoveCamera(Vector3 newPos, Vector3 newLookAt, float newFOV, float moveSecs)
	{
        ModeMovingToPos mode = (ModeMovingToPos)SetMode(CamModeID.kMoveToPos);
        mode.init(this, newPos, newLookAt, newFOV, moveSecs);
	}	
    
	public void MoveCameraToTarget(GameObject target, float finalDist , float finalHeight, float moveSecs, float closeEnough = kZeroDist)
	{
        ModeMovingToTarget mode = (ModeMovingToTarget)SetMode(CamModeID.kMoveToTarget);
        mode.init(this, target, finalDist, finalHeight, moveSecs, closeEnough);
	}	

    // Maintain current distance from object and current height. >0 is CCW
    // Look at target's pos plus ofset
    public void StartOrbit(GameObject target, float degPerSec, Vector3 offset)
    {
        ModeOrbit mode = (ModeOrbit)SetMode(CamModeID.kOrbit);
        mode.init(this, target, degPerSec, offset);
    }       

    public void StartBikeMode(GameObject qb)
    {
        ModeBikeView mode = (ModeBikeView)SetMode(CamModeID.kBikeView);
        mode.init(this, qb);        
    }    

    // public void StartSweep(GameObject target, float degPerSec, Vector3 offset, float leftAngDeg, float rightAngDeg)
    // {
    //     ModeSweep mode = (ModeSweep)SetMode(CamModeID.kSweep);
    //     mode.init(this, target, degPerSec, offset, leftAngDeg, rightAngDeg);
    // }           
    
//    public void StartBallChaseMode(FBBall ball)
//     {
//         ModeBallChase mode = (ModeBallChase)SetMode(CamModeID.kBallChase);
//         mode.init(this, ball);        
//     }    


	public void SendCmd(int cmd, object param)
	{
        if (_curMode != null)
            _curMode.handleCmd(cmd, param );  		
	}
    
	public CamModeID getMode()
    {
        return _curModeID;
    }
    
}
