using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class FeGround : MonoBehaviour
{
    public Ground beGround = null;

    protected List<GameObject> markerList = null; // makes it easier to track/delete them

    public GameObject markerPrefab;

    void Awake() 
    {
        beGround = new Ground();
        markerList = new List<GameObject>();        
    }
    void Start()
    {
    }

    // // Update is called once per frame
    // void Update()
    // {
    //     // Assume that if it's in the active list it's not nulll
    //     // If secsLeft runs out then remove it.
    //     int removed = activePlaces.RemoveAll( p => {
    //             p.secsLeft -= GameTime.DeltaTime();
    //             if (p.secsLeft <= 0)
    //                 RecyclePlace(p);
    //             return p.secsLeft <= 0; // remove from active list
    //     });
    //     //if (removed > 0)
    //     //    Debug.Log(string.Format("--- Removed {0} places --- {1} still active --- {2} free -------------------", removed, activePlaces.Count, freePlaces.Count));
    // }

    // protected void RecyclePlace(Place p){
    //     p.bike = null;                 
    //     p.marker.SetActive(false);
    //     freePlaces.Push(p); // add to free list
    //     placeArray[p.xIdx, p.zIdx] = null;
    // }

    // public void ClearPlaces()
    // {
    //     markerList.RemoveAll(m => {Object.Destroy(m); return true;});
    //     // is this asking too much of the GC?
    //     placeArray = new Place[pointsPerAxis,pointsPerAxis];
    //     activePlaces = new List<Place>();
    //     freePlaces = new Stack<Place>();         
    // }

    // public void RemovePlacesForBike(FrontendBike bike)
    // {
    //     activePlaces.RemoveAll( p => {
    //             if (p.bike == bike)
    //                 RecyclePlace(p);
    //             return p.bike == null; // remove from active list
    //     });        
    // }

    // public Place GetPlace(Vector3 pos)
    // {
    //     Vector3 gridPos = NearestGridPoint(pos);
    //     int xIdx = (int)Mathf.Floor((gridPos.x - minX) / gridSize );
    //     int zIdx = (int)Mathf.Floor((gridPos.z - minZ) / gridSize );
    //     //Debug.Log(string.Format("gridPos: {0}, xIdx: {1}, zIdx: {2}", gridPos, xIdx, zIdx));
    //     return IndicesAreOnMap(xIdx,zIdx) ? placeArray[xIdx,zIdx] : null;
    // }

    // public Place ClaimPlace(FrontendBike bike, Vector3 pos)
    // {
    //     Vector3 gridPos = NearestGridPoint(pos);        
    //     int xIdx = (int)Mathf.Floor((gridPos.x - minX) / gridSize );
    //     int zIdx = (int)Mathf.Floor((gridPos.z - minZ) / gridSize );

    //     Place p = IndicesAreOnMap(xIdx,zIdx) ? ( placeArray[xIdx,zIdx] ?? SetupPlace(bike, xIdx, zIdx) ) : null;
    //     // TODO: Should claiming a place already held by team reset the timer?
    //     return (p?.bike == bike) ? p : null;
    // }

    public static Vector3 NearestGridPoint(Vector3 pos) 
    {
        // Backend Ground has a 2d version of this
        float invGridSize = 1.0f / Ground.gridSize;
        return new Vector3( Mathf.Round(pos.x * invGridSize) * Ground.gridSize, pos.y, Mathf.Round(pos.z * invGridSize) * Ground.gridSize);
    } 

    // // Set up a place instance for use or re-use
    // protected Place SetupPlace(FrontendBike bike, int xIdx, int zIdx)
    // {
    //     Place p = freePlaces.Count > 0 ? freePlaces.Pop() : new Place(); 
    //     // Maybe populating a new one, maybe re-populating a used one.
    //     p.secsLeft = secsHeld;
    //     p.xIdx = xIdx;
    //     p.zIdx = zIdx;
    //     p.bike = bike;
    //     p.marker = SetupMarkerForPlace(p); // might create it
    //     placeArray[xIdx, zIdx] = p;
    //     activePlaces.Add(p);
    //     return p;
    // }

    // protected GameObject SetupMarkerForPlace(Place p)
    // {
    //     GameObject  marker = p.marker;
    //     if (marker == null) {
    //         marker = GameObject.Instantiate(markerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
    //         marker.transform.parent = transform;            
    //         markerList.Add(marker);
    //     }
    //     marker.transform.position = p.GetPos();
    //     GroundMarker gm = (GroundMarker)marker.transform.GetComponent("GroundMarker");
	// 	gm.SetColor(utils.hexToColor(p.bike.player.Team.Color));	
    //     marker.SetActive(true);	
    //     return marker;
    // }

    // public bool PointIsOnMap(Vector3 pt)
    // {
    //     int xIdx = (int)Mathf.Floor((pt.x - minX) / gridSize );
    //     int zIdx = (int)Mathf.Floor((pt.z - minZ) / gridSize );        
    //     return IndicesAreOnMap(xIdx, zIdx);
    // }
    
    // public bool IndicesAreOnMap(int xIdx, int zIdx)
    // {
    //     return !(xIdx < 0 || zIdx < 0 || xIdx >= pointsPerAxis || zIdx >= pointsPerAxis);
    // }
}
