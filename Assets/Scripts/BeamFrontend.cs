using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeamBackend;

public class BeamFrontend : MonoBehaviour, IBeamFrontend
{
	public FeGround feGround;

    protected Dictionary<string, GameObject> feBikes;

    protected BeamMain mainObj;

    protected BeamModeHelper feModeHelper;

    // Start is called before the first frame update
    void Start()
    {
        mainObj = BeamMain.GetInstance();
        feModeHelper = new BeamModeHelper(mainObj);
        feBikes = new Dictionary<string, GameObject>(); 
    }

	public  int BikeCount() => feBikes.Count;  


    public GameObject GetBikeObj(string bikeId)
    {
        try {
            return feBikes[bikeId];
        } catch (KeyNotFoundException) {
            return null;
        }
    }

    public List<GameObject> GetBikeList()
    {
        return feBikes.Values.ToList();
    }

    public GameObject GetBikeObjByIndex(int idx)
    {
        return feBikes.Values.ElementAt(idx);
    }    
    //
    // IBeamFrontend API
    //   

    // Backend game modes
    public IFrontendModeHelper ModeHelper() => (IFrontendModeHelper)feModeHelper;

    // Players
    public void OnNewPlayer(Player p)
    {
        UnityEngine.Debug.Log("FE.OnNewPlayer() currently does nothing");
    }

    public void OnClearPlayers()
    {
        UnityEngine.Debug.Log("FE.OnClearPlayers() currently does nothing");
    }

    // Bikes
    public void OnNewBike(IBike ib)
    {
        GameObject bikeGo = FrontendBikeFactory.CreateBike(ib, feGround);
        if (ib.player.IsLocal == true)
            mainObj.inputDispatch.SetLocalPlayerBike(bikeGo);
        feBikes[ib.bikeId] = bikeGo;
         mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard")?.SendMessage("AddBike", bikeGo);
    }
    public void OnBikeRemoved(string bikeId, bool doExplode)
    {
        GameObject go = feBikes[bikeId];
        feBikes.Remove(bikeId);
        mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard")?.SendMessage("RemoveBike", go);
		// if (inputDispatch.localPlayerBike != null && bikeObj == inputDispatch.localPlayerBike.gameObject)
		// {
		// 	Debug.Log("Boom! Player");
		// 	uiCamera.CurrentStage().transform.Find("RestartCtrl")?.SendMessage("moveOnScreen", null); 
		// }
		GameObject.Instantiate(mainObj.boomPrefab, go.transform.position, Quaternion.identity);
		UnityEngine.Object.Destroy(go);        
    }  
    public void OnClearBikes()
    {
		foreach (GameObject bk in feBikes.Values)
		{
			GameObject.Destroy(bk);
		}
		feBikes.Clear();
    }    

    public void OnBikeAtPlace(string bikeId, Ground.Place place, bool justClaimed)
    {        
        GetBikeObj(bikeId)?.GetComponent<FrontendBike>().OnBikeAtPlace(place, justClaimed);
    }    

    // Ground
    public void SetupPlaceMarker(Ground.Place p)
    {         
        feGround.SetupMarkerForPlace(p);    
    }
    public void OnFreePlace(Ground.Place p)
    {
        feGround.FreePlaceMarker(p);                   
    }        
    public void OnClearPlaces()
    {
        feGround.ClearMarkers();
    }

}
