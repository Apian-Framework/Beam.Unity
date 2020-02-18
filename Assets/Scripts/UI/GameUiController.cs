using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameUiController : MonoBehaviour
{
	public GameObject[] stages; // these are "sets" for the game mode UIs

	public ToastMgr toastMgr;
	public GameObject dbgFpsDisp;
    protected int _curStageIdx = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (dbgFpsDisp != null)
		{
			dbgFpsDisp.GetComponent<TextMeshProUGUI>().text =  string.Format("FPS: {0:0.0}", (1.0f / Time.smoothDeltaTime));
		}        
    }


	public void switchToNamedStage(string stageName)
	{
		_curStageIdx = -1;
		for(int i = 0; i< stages.Length; i++ )
		{
			if(stages[i].name.Equals(stageName))
			{
            	_curStageIdx = i;
				stages[i].SetActive(true);
			} else {
				stages[i].SetActive(false);
			}

		}		
		
		if (_curStageIdx == -1)
			Debug.LogWarning($"No stage named: {stageName}");

	}    

    public GameObject CurrentStage()
    {
        return stages[_curStageIdx];    
    }    

    public void ShowToast(string msg, Toast.ToastColor color=Toast.ToastColor.kBlue)
	{
		toastMgr.ShowToast(msg,color);
	}    
}
