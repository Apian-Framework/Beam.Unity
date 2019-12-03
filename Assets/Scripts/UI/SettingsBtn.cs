using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class SettingsBtn : UIBtn
{
    public UnityEvent onClick;
    // Start is called before the first frame update
    protected override void Start()
    {
		base.Start();	        
    }

    // Update is called once per frame
    protected override void Update()
    {
		base.Update();	        
    }

	public override void doSelect()
	{
        Debug.Log(string.Format("FrontendBike.Start()"));          
        onClick.Invoke();
	}    
}
