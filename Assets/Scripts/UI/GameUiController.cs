using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUiController : MonoBehaviour
{
	public GameObject[] stages; // these are "sets" for the game mode UIs

	protected ToastMgr _toastMgr;
    protected int _curStageIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
		_toastMgr  = (ToastMgr)transform.Find("ToastMgr").GetComponent<ToastMgr>();        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	public void switchToNamedStage(string stageName)
	{
        int targetStageIdx = -1;
		for(int i = 0; i< stages.Length; i++ )
		{
			if(stages[i].name.Equals(stageName))
			{
				targetStageIdx = i;
				break;
			}
		}		
		
		if (targetStageIdx != -1)
		{            
            _curStageIdx = targetStageIdx;
			stages[targetStageIdx].SetActive(true);
		}
			
	}    

    public GameObject CurrentStage()
    {
        return stages[_curStageIdx];    
    }    

    public void ShowToast(string msg, Toast.Color color=Toast.Color.kBlue)
	{
		_toastMgr.ShowToast(msg,color);
	}    
}
