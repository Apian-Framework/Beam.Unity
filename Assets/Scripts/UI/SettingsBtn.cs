using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsBtn : UIBtn
{
    public GameObject target;
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
		target.transform.GetComponent<MovableUISetItem>().toggle();
	}    
}
