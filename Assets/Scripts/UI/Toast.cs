using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;

public class Toast : MonoBehaviour
 {

    public enum ToastColor
    {
        kGreen = 0, //
        kBlue = 1, //
        kOrange = 2, //
        kRed = 3
    };

	public Color greenColor;
	public Color blueColor;
	public Color orangeColor;
	public Color redColor;

	public float switchSecs = .125f;
	public Vector3 offScreenPos;
	public Vector3 onScreenPos;
	public float height;
	public float vMargin = 2;
	public string toastTag; // toasts will overwrite other toasts with the the same tag

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

	public void Setup(ToastMgr _mgr, string msg, Toast.ToastColor color, float displaySecs, string tag)
	{
		mgr = _mgr;
		toastTag = tag;
		SetColor(color);
		SetText(msg);
		SetTimeout(displaySecs);
		SetIndexPos(0);
	}

	public void SetIndex(int idx)
	{
		bMoving = true;
		targetPos = onScreenPos + new Vector3(0, -idx*height, 0);
	}

	public void SetIndexPos(float dy)
	{
		bMoving = true;
		targetPos = onScreenPos + new Vector3(0, -dy, 0);
	}

	public void SetColor(Toast.ToastColor color)
	{
		Color rgb;
		switch(color)
		{
		case Toast.ToastColor.kOrange:
			rgb = orangeColor;
			break;
		case Toast.ToastColor.kRed:
			rgb = redColor;
			break;
		case Toast.ToastColor.kGreen:
			rgb = greenColor;
			break;
		case Toast.ToastColor.kBlue:
		default:
			rgb = blueColor;
			break;
		}

    	gameObject.transform.GetComponent<Image>().color = rgb; // vertex colors - so material doesn't change
	}
	public void SetText(string msg)
	{
		 TMP_Text tmpt = gameObject.transform.Find("Text").GetComponent<TMP_Text>();
		 tmpt.text = msg;
		 tmpt.ForceMeshUpdate();
		 height = tmpt.renderedHeight + vMargin*2;
		 RectTransform rt = GetComponent<RectTransform>();
		 Vector2 sd = rt.sizeDelta;
		 sd.y = height;
		 rt.sizeDelta = sd;
	}


	// public void SetText(string msg)
	// {
	// 	 gameObject.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = msg;
	// }

	public void SetTimeout( float secs)
	{
		secsLeft = secs;
	}

	public void DoSelect()
	{
		mgr?.RemoveToast(this);
	}
}
