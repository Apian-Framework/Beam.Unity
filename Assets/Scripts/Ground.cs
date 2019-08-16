using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    // North is Z, East is X,  Y is up
    public static float gridSize = 10f; // assume a square grid    
    public static float minX = -500f;
    public static float maxX = 500f;
    public static float minZ = -500f;
    public static float maxZ = 500f;

    public static readonly int pointsPerAxis = 101;

    public static Vector3 zeroPos = new Vector3(0f, 0f, 0f);

    public static float secsHeld = 15; // TODO: Maybe should ber per-bike and increase with time? Bike Trail FX would have to as well.

    public class Place
    {
        public int xIdx; // x index into array.
        public int zIdx;
        public Bike bike;
        public float secsLeft;
        public GameObject marker;

        public Vector3 GetPos()
        {
            return new Vector3(xIdx*gridSize+minX, 0, zIdx*gridSize+minZ);
        }
    }

    public Place[,] placeArray = null; 

    protected List<Place> activePlaces = null;
    protected Stack<Place> freePlaces = null; // re-use released/expired ones

    protected List<GameObject> markerList = null; // makes it easier to track/delete them

    public GameObject markerPrefab;

    void Awake() 
    {
        markerList = new List<GameObject>();        
        ClearPlaces();
    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Assume that if it's in the active list it's not nulll
        // If secsLeft runs out then remove it.
        int removed = activePlaces.RemoveAll( p => {
                p.secsLeft -= GameTime.DeltaTime();
                if (p.secsLeft <= 0)
                    RecyclePlace(p);
                return p.secsLeft <= 0; // remove from active list
        });
        //if (removed > 0)
        //    Debug.Log(string.Format("--- Removed {0} places --- {1} still active --- {2} free -------------------", removed, activePlaces.Count, freePlaces.Count));
    }

    protected void RecyclePlace(Place p){
        p.bike = null;                 
        p.marker.SetActive(false);
        freePlaces.Push(p); // add to free list
        placeArray[p.xIdx, p.zIdx] = null;
    }

    public void ClearPlaces()
    {
        markerList.RemoveAll(m => {Object.Destroy(m); return true;});
        // is this asking too much of the GC?
        placeArray = new Place[pointsPerAxis,pointsPerAxis];
        activePlaces = new List<Place>();
        freePlaces = new Stack<Place>();         
    }

    public void RemovePlacesForBike(Bike bike)
    {
        activePlaces.RemoveAll( p => {
                if (p.bike == bike)
                    RecyclePlace(p);
                return p.bike == null; // remove from active list
        });        
    }

    public Place GetPlace(Vector3 pos)
    {
        Vector3 gridPos = NearestGridPoint(pos);
        int xIdx = (int)Mathf.Floor((gridPos.x - minX) / gridSize );
        int zIdx = (int)Mathf.Floor((gridPos.z - minZ) / gridSize );
        //Debug.Log(string.Format("gridPos: {0}, xIdx: {1}, zIdx: {2}", gridPos, xIdx, zIdx));
        return IndicesAreOnMap(xIdx,zIdx) ? placeArray[xIdx,zIdx] : null;
    }

    public Place ClaimPlace(Bike bike, Vector3 pos)
    {
        Vector3 gridPos = NearestGridPoint(pos);        
        int xIdx = (int)Mathf.Floor((gridPos.x - minX) / gridSize );
        int zIdx = (int)Mathf.Floor((gridPos.z - minZ) / gridSize );

        Place p = IndicesAreOnMap(xIdx,zIdx) ? ( placeArray[xIdx,zIdx] ?? SetupPlace(bike, xIdx, zIdx) ) : null;
        // TODO: Should claiming a place already held by team reset the timer?
        return (p?.bike == bike) ? p : null;
    }

    public static Vector3 NearestGridPoint(Vector3 pos) 
    {
        float invGridSize = 1.0f / gridSize;
        return new Vector3( Mathf.Round(pos.x * invGridSize) * gridSize, pos.y, Mathf.Round(pos.z * invGridSize) * gridSize);
    } 

    // Set up a place instance for use or re-use
    protected Place SetupPlace(Bike bike, int xIdx, int zIdx)
    {
        Place p = freePlaces.Count > 0 ? freePlaces.Pop() : new Place(); 
        // Maybe populating a new one, maybe re-populating a used one.
        p.secsLeft = secsHeld;
        p.xIdx = xIdx;
        p.zIdx = zIdx;
        p.bike = bike;
        p.marker = SetupMarkerForPlace(p); // might create it
        placeArray[xIdx, zIdx] = p;
        activePlaces.Add(p);
        return p;
    }

    protected GameObject SetupMarkerForPlace(Place p)
    {
        GameObject  marker = p.marker;
        if (marker == null) {
            marker = GameObject.Instantiate(markerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            marker.transform.parent = transform;            
            markerList.Add(marker);
        }
        marker.transform.position = p.GetPos();
        GroundMarker gm = (GroundMarker)marker.transform.GetComponent("GroundMarker");
		gm.SetColor(p.bike.player.Team.Color);	
        marker.SetActive(true);	
        return marker;
    }

    protected bool IndicesAreOnMap(int xIdx, int zIdx)
    {
        return !(xIdx < 0 || zIdx < 0 || xIdx >= pointsPerAxis || zIdx >= pointsPerAxis);
    }
}
