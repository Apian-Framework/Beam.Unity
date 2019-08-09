using UnityEngine;
using System.Collections;


public class GameTime : MonoBehaviour 
{
    protected class GameClock
    {
        // Clock info displayed time = (ActualTime - _realTimeBase) * _timeMult + _timeOffset    
        float    _realTimeBase;
        float    _timeMult;
        float    _timeOffset;    
    
        float    _rateAtPause; // saved so "Resume" can just put the rate back
        bool     _isPaused;
        
        public GameClock()
        {
            SetRate (1.0f);
            _isPaused = false;
            _rateAtPause = 1.0f;
        }
        
        public void SetRate(float rate)
        {
            float curTime = GetTime();
            _realTimeBase = Time.time;
            _timeOffset = curTime - _realTimeBase;
            _timeMult = rate;           
        }
        
        public float GetTime()
        {
            return (Time.time - _realTimeBase) * _timeMult + _timeOffset;  
        }
        
        public float DeltaTime()
        {
            return Time.deltaTime * _timeMult;    
        }
        
        public void Pause()
        {
            if (!_isPaused)
            {
                _rateAtPause = _timeMult;
                SetRate(0);
                _isPaused = true;
            }        
        }
        
        public void Resume()
        {
            if (_isPaused)
            {
                SetRate(_rateAtPause);
                _rateAtPause = 1.0f; // not really necessary
                _isPaused = false;         
            }          
        }
        
        public bool IsPaused()
        {
            return _isPaused;    
        }
        
        public float GetRate()
        {
            return _timeMult;    
        }
        
    }
    
    static protected GameClock gc = null;
    
	// Use this for initialization
	void Start () 
    {
	    gc = new GameClock();
	}
	
    void OnDestroy()
    {
        gc = null;
    }
    
	// Update is called once per frame
	//void Update () {}
    
    static public float GetTime()
    {
        return gc.GetTime();   
    }
    
    static public float DeltaTime()
    {
        return gc.DeltaTime();
    }
    
    static public void SetRate(float newRate)
    {
        gc.SetRate(newRate);
    }
    
    static public void Pause()
    {
        gc.Pause();   
    }
    
    static public void Resume()
    {
        gc.Resume();
    }
    
    static public bool IsPaused()
    {
        return gc.IsPaused();   
    }
    
    static public float GetRate()
    {
        return gc.GetRate();    
    }
    
}
