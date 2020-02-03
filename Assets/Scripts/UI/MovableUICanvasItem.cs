using UnityEngine;
using System.Collections;

public class MovableUICanvasItem : MonoBehaviour {
	
	public bool    startOnScreen;
	public float switchSecs = .3f;
	public Vector2 offScreenPos;
	public Vector2 onScreenPos;
	
	protected bool bMoving;
	protected Vector2 targetPos;
	protected Vector2 curVel = Vector2.zero;

	protected RectTransform rectTransform;
	
	public const float kZeroDist = .01f;
	
	// Use this for initialization
	protected virtual void Start () 
	{
		bMoving = false;
		rectTransform = GetComponent<RectTransform>();
		
		if (startOnScreen)
			rectTransform.anchoredPosition = onScreenPos;	
		else
			rectTransform.anchoredPosition = offScreenPos;				
	}
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		if (bMoving)
		{
			
			rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition,targetPos,ref curVel,switchSecs);
			
			if (Vector3.Distance(rectTransform.anchoredPosition, targetPos) < kZeroDist)
			{
				bMoving = false;
				curVel = Vector2.zero;
			}
		}			
	}
	
	public void toggle()
	{
		bMoving = true;
		targetPos = targetPos == onScreenPos ? offScreenPos : onScreenPos;
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
		rectTransform.anchoredPosition = offScreenPos;
		bMoving = false;
	}
	

}
