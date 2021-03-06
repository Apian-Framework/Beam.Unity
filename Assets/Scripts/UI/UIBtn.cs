using UnityEngine;
using System.Collections;

public class UIBtn : MonoBehaviour {
	
	public float pressOffset = .25f;
	
	protected GameObject _hilight;
	protected GameObject _halo;	
	protected Vector3 _basePos; // except for x
	protected const float hiZSpeed = 15.0f;
	
	protected bool _bIsHighLit;
			
	
	// Use this for initialization
	protected virtual void Start () 
	{

		
		_hilight = transform.Find("hilight")?.gameObject;
		_halo = transform.Find("halo")?.gameObject;		
		
		_basePos = transform.position;		
		_bIsHighLit = false;			
	}
	
	
	public void setHighLit(bool doIt)
	{
		_bIsHighLit = doIt;
        if (_hilight != null)		
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

