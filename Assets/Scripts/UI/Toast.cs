using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class Toast : MonoBehaviour
 {
	
    public enum ToastColor 
    { 
        kBlue = 0, //
        kOrange = 1, //
        kRed = 2
    };

	public Color blueColor;
	public Color orangeColor;
	public Color redColor;

	public float switchSecs = .125f;
	public Vector3 offScreenPos;
	public Vector3 onScreenPos;
	public float height;
	
	public bool bMoving;
	public Vector3 targetPos;
	protected Vector3 curVel = Vector3.zero;

	protected ToastMgr mgr;
	protected float secsLeft;
	
	public const float kZeroDist = .01f;
	
	// Use this for initialization
	protected  void Start () 
	{
		// Note that this pretty much ignores all the UIBtn
   		transform.localPosition = offScreenPos;				
		bMoving = true;		   
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		float frameMs = GameTime.DeltaTime();
		if (bMoving)
		{
			transform.localPosition = Vector3.SmoothDamp(transform.localPosition,targetPos,ref curVel,switchSecs);
			if (Vector3.Distance(transform.localPosition, targetPos) < kZeroDist)
			{
				bMoving = false;
				curVel = Vector3.zero;
			}
		}	

		secsLeft -= frameMs;
		if (secsLeft <= 0)
		{
			mgr?.RemoveToast(this);
		}		
	}
		
	
	public void moveOffScreenNow()
	{
		transform.localPosition = offScreenPos;
		bMoving = false;
	}
	
	public void Setup(ToastMgr _mgr, string msg, Toast.ToastColor color, float displaySecs)
	{
		mgr = _mgr;
		SetColor(color);
		SetText(msg);
		SetTimeout(displaySecs);
		SetIndex(0);
	}

	public void SetIndex(int idx)
	{
		bMoving = true;
		targetPos = onScreenPos + new Vector3(0, -idx*height, 0);
	}

	public void SetColor(Toast.ToastColor color)
	{
		Color rgb =  blueColor;
		// Yuk
		if (color == Toast.ToastColor.kOrange)
			rgb = orangeColor;
		if (color == Toast.ToastColor.kRed)
			rgb = redColor;

    	gameObject.transform.GetComponent<Image>().color = rgb;
	}
	public void SetText(string msg)
	{
		gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = msg;
	}

	public void SetTimeout( float secs)
	{
		secsLeft = secs;
	}

	public void DoSelect()
	{            
		mgr?.RemoveToast(this);
	}   	
}
