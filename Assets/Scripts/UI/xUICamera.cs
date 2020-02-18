using UnityEngine;
using TMPro;
using System.Collections;

public class xUICamera : MonoBehaviour {
	
		
	public GameObject[] stages; // these are "sets". We want to go to the "UICamPos" for each
	
	public float switchSecs = .5f;
	
	protected int targetStageIdx;	
	protected float totalMoveTime;
	protected Vector3 curVel;
	protected bool bSwitching;	
    protected float switchElapsedSecs;
	
	protected LayerMask  _btnLayerMask; // set in Start layer	
	protected const float kZeroDist = .1f;
	
	public delegate void CaptureSwipeDelegate(bool bIsDone); // != bIsDone means it's started.	
	
	protected TextMeshPro _dbgFPSMesh = null;

	protected ToastMgr _toastMgr;
	
	public LayerMask getBtnLayerMask()
	{
		return _btnLayerMask;	
	}
	
	

	Vector3 stagePos(int idx)
	{
		// Really? You gotta do all this?
		GameObject cam = (GameObject)stages[idx].transform.Find("UICamPos").gameObject;	
		return cam.transform.position;
	}	
	
	protected void disableAllStagesExcept(int enabledIdx)
	{
		for(int i = 0; i< stages.Length; i++ )
		{
			stages[i].SetActive(i == enabledIdx);	
		}
	}
	
		
	protected void setInitialStage()
	{
		disableAllStagesExcept(0);
		transform.position = stagePos(0);
		targetStageIdx = 0;
		bSwitching = false;			
		
	}
	
	protected void updateStageSwitch()
	{
		if (bSwitching)
		{
			Vector3 targetPos = stagePos(targetStageIdx);
			
			transform.position = Vector3.SmoothDamp(transform.position,targetPos,ref curVel,switchSecs);
			
            switchElapsedSecs += Time.deltaTime;
            if (switchElapsedSecs > switchSecs)
                disableAllStagesExcept(targetStageIdx);
      
			if (Vector3.Distance(transform.position, targetPos) < kZeroDist)
			{
				bSwitching = false;
				disableAllStagesExcept(targetStageIdx);
				curVel = Vector3.zero;
			}
		}		
	}
	
	

	// Use this for initialization

	void Start() 
	{
        // &&& Dumb hcak - might not even do anything
        // Make em start so they all call Start() - then they'll get disabled
        foreach(GameObject go in stages)
            go.SetActive(true);       
        
        _btnLayerMask = (1 << gameObject.layer);      
        setInitialStage();
        setState( );           
        
		_dbgFPSMesh = (TextMeshPro)transform.Find("fpsdisp").GetComponent<TextMeshPro>(); // might or might not be there
		_toastMgr  = (ToastMgr)transform.Find("ToastMgr").GetComponent<ToastMgr>();
	}
	

	// Update is called once per frame
	void Update () {
		
		updateStageSwitch();
		
		
		if (_dbgFPSMesh != null)
		{
			_dbgFPSMesh.text =  string.Format("FPS: {0:0.0}", (1.0f / Time.smoothDeltaTime));
			
		}

	}
	
	void FixedUpdate () 
	{


	}	
	
	public void switchToStage(GameObject dest)
	{
		
		targetStageIdx = -1;
		for(int i = 0; i< stages.Length; i++ )
		{
			if(stages[i] == dest)
			{
				targetStageIdx = i;
				break;
			}
		}		
		
		if (targetStageIdx != -1)
		{
			bSwitching = true;	
            switchElapsedSecs = 0;
			stages[targetStageIdx].SetActive(true);
		}
	}	
	

	
	public void switchToNamedStage(string stageName)
	{
		targetStageIdx = -1;
		for(int i = 0; i< stages.Length; i++ )
		{
			if(stages[i].name.Equals(stageName))
			{
				targetStageIdx = i;
				break;
			}
		}		
		
		if (targetStageIdx != -1)
		{
			bSwitching = true;	
            switchElapsedSecs = 0;            
			stages[targetStageIdx].SetActive(true);
		}
			
	}
	
    public void setToNamedStage(string stageName)
 
    {
        targetStageIdx = -1;
        for(int i = 0; i< stages.Length; i++ )
        {
            if(stages[i].name.Equals(stageName))
            {
                 targetStageIdx = i;
                break;
            }
        }       
     
        if (targetStageIdx != -1)
        {
            bSwitching = false;      
            disableAllStagesExcept(targetStageIdx);
            Vector3 targetPos = stagePos(targetStageIdx);
            transform.position = targetPos;
            
        } 
 }
    
	
	// This is ugly. If the state classes  are to beuse they should at least get created and stashed
	// when not active - that would a) reduce alloc/free overhead and b) allow push/pop pause and resume.
	public void setState()
	{
		
	}

    public void CancelTaps()
    {

    }
    
    public GameObject CurrentStage()
    {
        return stages[targetStageIdx];    
    }
	
    public void ShowToast(string msg, Toast.ToastColor color=Toast.ToastColor.kBlue)
	{
		_toastMgr.ShowToast(msg,color);
	}
}
