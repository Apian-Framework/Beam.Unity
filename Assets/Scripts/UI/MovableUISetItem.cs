using UnityEngine;
using System.Collections;

public class MovableUISetItem : MonoBehaviour {
	
	public bool    startOnScreen;
	public float switchSecs = .3f;
	public Vector3 offScreenPos;
	public Vector3 onScreenPos;
	
	protected bool bMoving;
	protected Vector3 targetPos;
	protected Vector3 curVel = Vector3.zero;

	
	public const float kZeroDist = .01f;
	
	// Use this for initialization
	protected virtual void Start () 
	{
		bMoving = false;
		
		if (startOnScreen)
			transform.localPosition = onScreenPos;	
		else
			transform.localPosition = offScreenPos;				
	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		if (bMoving)
		{
			
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition,targetPos,ref curVel,switchSecs);
			
			if (Vector3.Distance(transform.localPosition, targetPos) < kZeroDist)
			{
				bMoving = false;
				curVel = Vector3.zero;
			}
		}			
	}
	
	public void moveOnScreen()
	{
		bMoving = true;
		targetPos = onScreenPos;
	}
	
	public void moveOffScreen()
	{
		bMoving = true;
		targetPos = offScreenPos;
	}
	
	public void moveOffScreenNow()
	{
		transform.localPosition = offScreenPos;
		bMoving = false;
	}
	

}
