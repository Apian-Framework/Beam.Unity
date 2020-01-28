using UnityEngine;
using System.Collections;

public abstract class UICamState
{	
	public UICamera _uiCam;
    public BeamMain _main = null;    
    
    // buttons and stuff
    protected bool bUIPressed;
    UIBtn curBtn = null;        
    
    // returns true if it ate a press
    protected bool handleButtons()
    {
        bool eventEaten = false;
        RaycastHit hit;
        Ray ray;
     
        bool btnDown = Input.GetMouseButton(0);
     
        if (!bUIPressed)
        {
            // A finger was NOT already down...
            
            if (btnDown)
            {       
                // But it is now...                
                ray = _uiCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, _uiCam.GetComponent<Camera>().farClipPlane, _uiCam.getBtnLayerMask()))
                {
                    // It hit SOMETHING in the ui layer
                    
                    // Is it over a button?
                    UIBtn btn = (UIBtn)hit.transform.GetComponent<UIBtn>();
                    if (btn)
                    {
                        btn.setHighLit(true);
                        curBtn = btn;
                        bUIPressed = true;                        
                    }
                    
                    // Note if it hits something we don't know about then we DON'T set UIPressed
                    // to keep a tap from reporting

                   eventEaten = true; // but we DO eat the event to keep a generic tap from reporting
                    
                }   
            }
        }
        else
        {
            // Finger was down to start with...
            if (!btnDown)
            {
                // And it's not now...
                
                if (bUIPressed)
                {
                    // we know about a press
                    if (curBtn)
                    {
                        // It was a button
                    
                        // check to see if still over
                        ray = _uiCam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit, _uiCam.GetComponent<Camera>().farClipPlane, _uiCam.getBtnLayerMask()))
                        {
                            // yes - tell the button to do its thing
                            UIBtn btn = (UIBtn)hit.transform.GetComponent<UIBtn>();
                            if (btn == curBtn)
                            {
                                Debug.Log("YOW!!!!");
                                // It's still over
                                curBtn.doSelect();
                            }
                            eventEaten = true; // &&& maybe shoud be in curbtn check?
                        }                   
                 
                        // in any case deactivate the button
                        curBtn.setHighLit(false);
                        curBtn = null;      
                    }                   
                }
                bUIPressed = false; // In any case, with the mouse button up UI can;t be down.
            }
         
        }   
        return eventEaten;
    }       
	
    public virtual void init(UICamera pc)
    {
        _uiCam = pc;
        _main = BeamMain.GetInstance();             
        curBtn = null;
        bUIPressed = false;  
    }
    
    public virtual void CancelTaps() // cancel any pending UI
    {
        
    }

	public virtual void update() {}
	public virtual void fixedUpdate() {}
	public virtual void end() {}
	
};	


// Wait for button presses. Handle 'em
public class StateIdle : UICamState
{		
    protected bool bTapDown = false;
    
    protected void LookForTap()
    {
        bool btnDown = Input.GetMouseButtonDown(0);
        bool btnUp = Input.GetMouseButtonUp(0);
        
        if (!bTapDown)
        {
            // A finger was NOT already down...    
            if (btnDown)
            {       
                // But it is now...                
                _main.HandleTap(true); // true is down
                bTapDown = true;
            }
        }
        else
        {
            // Finger was down to start with...
            if (btnUp)
            {
                // And it's not now...            
                if (bTapDown)
                {
                    // we know about a press
                    _main.HandleTap(false); // true is down 
                }
                bTapDown = false; // In any case, with the mouse button up UI can;t be down.
            }
        }   
    }     
    
	public override void init(UICamera pc) 
	{
		base.init(pc);
        
        bTapDown = false;
    }
 
    
    
	public override void update() 
	{
		bool eaten = handleButtons();
        
        if (eaten)
            bTapDown = false;
        else
            LookForTap();
        
	}
	
	public override void end() 
	{
		
	}	
	
}




