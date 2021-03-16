using UnityEngine;
using System.Collections;

public class MovableUICanvasItem : MonoBehaviour {

	public bool    startOnScreen;
	public float switchSecs = .3f;
	public Vector2 offScreenPos;
	public Vector2 onScreenPos;

	public bool shouldBeOnScreen = false;
	protected bool bMoving;
	protected Vector2 curVel = Vector2.zero;
	protected RectTransform rectTransform;
	protected const float kZeroDist = .01f;
	protected Vector2 TargetPos => shouldBeOnScreen ? onScreenPos : offScreenPos;

	// Use this for initialization
	protected virtual void Start ()
	{
		bMoving = false;
		rectTransform = GetComponent<RectTransform>();

		if (startOnScreen)
			moveOnScreenNow();
		else
			moveOffScreenNow();
	}

	// Update is called once per frame
	protected virtual void Update ()
	{
		Vector2 targetPos = TargetPos;
		if (bMoving)
		{
			rectTransform.anchoredPosition = Vector2.SmoothDamp(rectTransform.anchoredPosition,targetPos,ref curVel,switchSecs);
			if (Vector2.Distance(rectTransform.anchoredPosition, targetPos) < kZeroDist)
			{
				bMoving = false;
				rectTransform.anchoredPosition= targetPos; // make it exact for the "not moving" test below
				curVel = Vector2.zero;
			}
		} else {
			// Positions must have changed
			if (!rectTransform.anchoredPosition.Equals(targetPos))
				bMoving = true;
		}
	}

	public void toggle()
	{
		bMoving = true;
		shouldBeOnScreen = !shouldBeOnScreen;
	}

	public void moveOnScreen()
	{
		shouldBeOnScreen = true;
		bMoving = true;
	}

	public void moveOffScreen()
	{
		shouldBeOnScreen = false;
		bMoving = true;
	}

	public void moveOnScreenNow()
	{
		rectTransform.anchoredPosition = onScreenPos;
		shouldBeOnScreen = true;
		bMoving = false;
	}
	public void moveOffScreenNow()
	{
		shouldBeOnScreen = false;
		rectTransform.anchoredPosition = offScreenPos;
		bMoving = false;
	}


}
