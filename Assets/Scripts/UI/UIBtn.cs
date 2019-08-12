using UnityEngine;
using System.Collections;

public class UIBtn : MonoBehaviour {
	
	public float pressOffset = .25f;
	
	protected GameObject _hilight;
	protected GameObject _halo;	
	protected Vector3 _basePos; // except for x
	protected const float hiZSpeed = 15.0f;
	
	protected bool _bIsHighLit;
	
	protected UICamera _uiCamScript;		
	
	// Use this for initialization
	protected virtual void Start () 
	{
		GameObject uiCamera = GameObject.Find("UICamera");
		
		_uiCamScript = (UICamera)uiCamera.GetComponent("UICamera");	
		
		_hilight = transform.Find("hilight").gameObject;
		_halo = transform.Find("halo").gameObject;		
		
		_basePos = transform.position;		
		_bIsHighLit = false;			
	}
	
	
	public void setHighLit(bool doIt)
	{
		_bIsHighLit = doIt;
		_hilight.SetActive(doIt);
        
        if (_halo != null)
		    _halo.SetActive(doIt);
	}
	
	
	// Update is called once per frame
	protected virtual void Update () 
	{
		Vector3 newPos = transform.position;
		float targetZ = _basePos.z - (_bIsHighLit ? pressOffset : 0);
		
		float zErr = targetZ - newPos.z;
		float dZ = hiZSpeed * Time.smoothDeltaTime;
		if ( dZ >= Mathf.Abs(zErr))
			dZ = zErr;
		else
			dZ *= Mathf.Sign(zErr);

		newPos.z += dZ;
		
		transform.position = newPos;
	}
	
	public virtual void doSelect()
	{
		// UICamera decides to call this	
	}
	
	
	
}


/*
		float z = _basePos.z;
		float xRange = 2.0f;
		
		_hilight.SetActive(false);		
		if ( Mathf.Abs(x) < xRange)
		{
			_hilight.SetActive(true);			
			float theta = (Mathf.PI / 2.0f ) * x  / xRange;
			float dZ = -2.0f * Mathf.Cos(theta);	
			z += dZ;
			newPos.z = z;
		}
*/

