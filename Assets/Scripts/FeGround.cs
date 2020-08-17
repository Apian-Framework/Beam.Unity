using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamGameCode;

public class FeGround : MonoBehaviour
{
    public Ground beGround = null;

    protected Dictionary<int, GameObject> activeMarkers;
    protected Stack<GameObject> idleMarkers;

    protected Dictionary<(int,int), GameObject> activeConnectors; // tuple is (posHash1, posHash2)
    protected Stack<GameObject> idleConnectors;

    public GameObject markerPrefab;
    public GameObject connectorPrefab;

    void Awake()
    {
        activeMarkers = new Dictionary<int, GameObject>();
        idleMarkers = new Stack<GameObject>();
        activeConnectors = new Dictionary<(int,int), GameObject>();
        idleConnectors =  new Stack<GameObject>();
    }

    public void ClearMarkers()
    {
        ClearConnectors();
        foreach (GameObject go in activeMarkers.Values.ToList().Union(idleMarkers))
            Object.Destroy(go);
        activeMarkers.Clear();
        idleMarkers.Clear();
    }

    public GameObject SetupMarkerForPlace(BeamPlace p)
    {
        int posHash = p.PosHash;
        GameObject marker = null;
        try {
            marker = activeMarkers[posHash];
        } catch(KeyNotFoundException) {
            marker = idleMarkers.Count > 0 ? idleMarkers.Pop() : GameObject.Instantiate(markerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            marker.transform.parent = transform;
            activeMarkers[posHash] =  marker;
        }
        marker.transform.position = utils.Vec3(p.GetPos());
        GroundMarker gm = (GroundMarker)marker.transform.GetComponent("GroundMarker");
		gm.SetColor(utils.hexToColor(p.bike.team.Color));
        marker.SetActive(true);
        return marker;
    }

    public void FreePlaceMarker(BeamPlace p)
    {
        int posHash = p.PosHash;
        try {
            GameObject marker = activeMarkers[posHash];

            FreeConnectorsForPlace(p);

            marker.SetActive(false);
            idleMarkers.Push(marker);
            activeMarkers.Remove(posHash);
        }  catch(KeyNotFoundException) { }
    }

    protected void FreeConnectorsForPlace(BeamPlace p1)
    {
        if (p1 == null)
            return;

        int h1 = p1.PosHash;
        foreach( int h2 in GetAdjacentPlaceHashes(p1) )
        {
            if (activeConnectors.ContainsKey((h1,h2)))
                FreeConnector(h1,h2);

            if (activeConnectors.ContainsKey((h2,h1)))
                FreeConnector(h2,h1);
        }

    }

    protected List<int> GetAdjacentPlaceHashes(BeamPlace place)
    {
        if (place == null)
            return null;
        int px = place.xIdx;
        int pz = place.zIdx;
        return new List<int>() { BeamPlace.MakePosHash(px, pz+1),
                                 BeamPlace.MakePosHash(px, pz-1),
                                 BeamPlace.MakePosHash(px+1, pz),
                                 BeamPlace.MakePosHash(px-1, pz) };
    }


    public void ClearConnectors()
    {
        foreach (GameObject go in activeConnectors.Values.ToList().Union(idleConnectors))
            Object.Destroy(go);
        activeConnectors.Clear();
        idleConnectors.Clear();
    }
    public void SetupConnector(BeamPlace p1, BeamPlace p2)
    {
        // Includes checking if one is already in place in either direction, so go ahead and call
        // instead of checking first
        int posHash1 = p1.PosHash;
        int posHash2 = p2.PosHash;
        GameObject connGO = null;
        if (   (activeConnectors.TryGetValue((posHash1,posHash2), out connGO) == false)
            && (activeConnectors.TryGetValue((posHash2,posHash1), out connGO) == false) )
        {
            // Not in active list. Reuse an idle one or create a new instance
            connGO = idleConnectors.Count > 0 ? idleConnectors.Pop() : GameObject.Instantiate(connectorPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            connGO.transform.parent = transform; // make a child of the ground obj
            Connector conn = (Connector)connGO.transform.GetComponent("Connector");
            conn.SetupForPlaces( p1, p2);

            activeConnectors[(posHash1,posHash2)] =  connGO; // add to active list

            connGO.SetActive(true);
        }
    }

    public void FreeConnector(int fromHash, int toHash)
    {
        try {
            GameObject connGO = activeConnectors[(fromHash, toHash)];
            connGO.SetActive(false);
            idleConnectors.Push(connGO);
            activeConnectors.Remove((fromHash, toHash));
        }  catch(KeyNotFoundException) { }
    }

}
