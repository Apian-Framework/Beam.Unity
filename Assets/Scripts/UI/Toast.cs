using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Toast : MonoBehaviour {
	
    public enum Color 
    { 
        kBlue, //
        kOrange, //
        kRed
    };

	public Material blueMaterial;
	public Material orangeMaterial;	
	public Material redMaterial;

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
	protected virtual void Start () 
	{
   		transform.localPosition = offScreenPos;				
		bMoving = true;		   
	}
	
	// Update is called once per frame
	protected virtual void Update () 
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
	
	public void Setup(ToastMgr _mgr, string msg, Toast.Color color, float displaySecs)
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

	public void SetColor(Toast.Color color)
	{
		Material mat = blueMaterial;
		// Yuk
		if (color == Toast.Color.kOrange)
			mat = orangeMaterial;
		if (color == Toast.Color.kRed)
			mat = redMaterial;

    	transform.GetComponent<Renderer>().material = mat;
	}

	public void SetText(string msg)
	{
		gameObject.transform.Find("Text").GetComponent<TextMeshPro>().text = msg;
	}

	public void SetTimeout( float secs)
	{
		secsLeft = secs;
	}
}
