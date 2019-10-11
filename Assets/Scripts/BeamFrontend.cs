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
    public void NewPlayer(Player p)
    {
        UnityEngine.Debug.Log("FE.NewPlayer() currently does nothing");
    }

    // Bikes
    public void NewBike(IBike ib)
    {
        GameObject bikeGo = FrontendBikeFactory.CreateBike(ib, feGround);
        feBikes[ib.bikeId] = bikeGo;
    }
    public void DestroyBike(string bikeId, bool doExplode)
    {
        Debug.Log("** Need to implement FE.DestroyBike()");
    }  
    public void DestroyBikes()
    {
		foreach (GameObject bk in feBikes.Values)
		{
			GameObject.Destroy(bk);
		}
		feBikes.Clear();
    }    

    public void ReportBikeAtPoint(string bikeId, Vector2 pos)
    {
        //Debug.Log(string.Format("** Reporting bike {0} at {1}", bikeId, pos));        
        feBikes[bikeId].GetComponent<FrontendBike>().DealWithPlace(pos);
    }    

    // Ground
    public void SetupPlaceMarker(Ground.Place p)
    {
        UnityEngine.Debug.Log("** Need to implement FE.SetupPlaceMarker()");     
    }
    public void FreePlaceMarker(Ground.Place p)
    {
        Debug.Log("** Need to implement FE.FreePlaceMarker()");           
    }        
    public void ClearPlaceMarkers()
    {
        feGround.ClearMarkers();
    }

}
