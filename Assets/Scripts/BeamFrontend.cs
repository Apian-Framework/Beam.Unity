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

    // Start is called before the first frame update
    void Start()
    {
        mainObj = BeamMain.GetInstance();
        feBikes = new Dictionary<string, GameObject>(); 
    }

	public  int BikeCount() => feBikes.Count;   

    public GameObject GetBikeObj(string bikeId)
    {
        return feBikes[bikeId];
    }

    public GameObject GetBikeObjByIndex(int idx)
    {
        return feBikes.Values.ElementAt(idx);
    }    
    //
    // IBeamFrontend API
    //   

    // Players
    public void OnNewPlayer(Player p)
    {
        UnityEngine.Debug.Log("FE.NewPlayer() currently does nothing");
    }

    // Bikes
    public void OnNewBike(IBike ib)
    {
        GameObject bikeGo = FrontendBikeFactory.CreateBike(ib, feGround);
        feBikes[ib.bikeId] = bikeGo;
    }
    public void OnBikeDestroyed(string bikeId, bool doExplode)
    {
        Debug.Log("** Need to implement FE.DestroyBike()");
    }  
    public void OnClearBikes()
    {
		foreach (GameObject bk in feBikes.Values)
		{
			GameObject.Destroy(bk);
		}
		feBikes.Clear();
    }    

    public void OnBikeAtPlace(string bikeId, Ground.Place place)
    {
        //Debug.Log(string.Format("** Reporting bike {0} at {1}", bikeId, pos));        
        feBikes[bikeId].GetComponent<FrontendBike>().OnBikeAtPlace(place);
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
