using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeamBackend;
using UniLog;

public class BeamFrontend : MonoBehaviour, IBeamFrontend
{
    public enum ModeCmds : int {
        kOnNewPeer,
        kOnPeerLeft,
        kOnPeersCleared,
        kOnNewBike,
        kOnBikeRemoved,
        kOnBikesCleared,
        kOnBikeAtPlace,
        kSetupPlaceMarker,
        kPlaceFreed,
        kPlacesCleared,
    }    
	public FeGround feGround;

    protected Dictionary<string, GameObject> feBikes;

    protected BeamMain mainObj;

    protected BeamUserSettings userSettings;

    protected BeamFeModeHelper _feModeHelper;

    public UniLogger logger;

    // Start is called before the first frame update
    void Start()
    {
        userSettings = BeamUserSettings.CreateDefault();
        mainObj = BeamMain.GetInstance();
        _feModeHelper = new BeamFeModeHelper(mainObj);
        feBikes = new Dictionary<string, GameObject>(); 
        logger = UniLogger.GetLogger("Frontend");
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

    public BeamUserSettings GetUserSettings() => userSettings;

    public void OnStartMode(int modeId, object param) =>  _feModeHelper.OnStartMode(modeId, param);
    public void OnEndMode(int modeId, object param) => _feModeHelper.OnEndMode(modeId, param);

    // Players
    public void OnNewPeer(BeamPeer p, int modeId)
    {
        logger.Info($"New Peer: {p.Name}, Id: {p.PeerId}");
    }

    public void OnPeerLeft(string p2pId, int modeId) 
    {
        logger.Info("Peer Left: {p2pId}");            
    }

    public void OnClearPeers(int modeId)
    {
        logger.Info("OnClearPeers() currently does nothing");
    }

    // Bikes
    public void OnNewBike(IBike ib, int modeId)
    {
        logger.Info($"FE.OnNewBike(). Id: {ib.bikeId}, LocalPlayer: {ib.ctrlType == BikeFactory.LocalPlayerCtrl}"); 
        GameObject bikeGo = FrontendBikeFactory.CreateBike(ib, feGround);
        if (ib.ctrlType == BikeFactory.LocalPlayerCtrl)
            mainObj.inputDispatch.SetLocalPlayerBike(bikeGo);
        feBikes[ib.bikeId] = bikeGo;
        mainObj.uiCamera.CurrentStage().transform.Find("Scoreboard")?.SendMessage("AddBike", bikeGo);
    }
    public void OnBikeRemoved(string bikeId, bool doExplode, int modeId)
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
    public void OnClearBikes(int modeId)
    {
		foreach (GameObject bk in feBikes.Values)
		{
			GameObject.Destroy(bk);
		}
		feBikes.Clear();
    }    

    public void OnBikeAtPlace(string bikeId, Ground.Place place, bool justClaimed, int modeId)
    {        
        GetBikeObj(bikeId)?.GetComponent<FrontendBike>().OnBikeAtPlace(place, justClaimed);
    }    

    // Ground
    public void SetupPlaceMarker(Ground.Place p, int modeId)
    {         
        feGround.SetupMarkerForPlace(p);    
    }
    public void OnFreePlace(Ground.Place p, int modeId)
    {
        feGround.FreePlaceMarker(p);                   
    }        
    public void OnClearPlaces(int modeId)
    {
        feGround.ClearMarkers();
    }

}
