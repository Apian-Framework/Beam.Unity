using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class FeGround : MonoBehaviour
{
    public Ground beGround = null;

    protected Dictionary<int, GameObject> activeMarkers;
    protected Stack<GameObject> idleMarkers;

    public GameObject markerPrefab;

    void Awake()
    {
        activeMarkers = new Dictionary<int, GameObject>();
        idleMarkers = new Stack<GameObject>();
    }

    public void ClearMarkers()
    {
        foreach (GameObject go in activeMarkers.Values.ToList().Union(idleMarkers))
            Object.Destroy(go);
        activeMarkers.Clear();
        idleMarkers.Clear();
    }

    public GameObject SetupMarkerForPlace(BeamPlace p)
    {
        int posHash = p.posHash();
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
        int posHash = p.posHash();
        GameObject marker = null;
        try {
            marker = activeMarkers[posHash];
            marker.SetActive(false);
            idleMarkers.Push(marker);
            activeMarkers.Remove(posHash);
        }  catch(KeyNotFoundException) { }
    }
}
