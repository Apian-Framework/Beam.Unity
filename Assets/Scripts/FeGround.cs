using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class FeGround : MonoBehaviour
{
    public Ground beGround = null;

    protected Dictionary<string, GameObject> markers; 

    public GameObject markerPrefab;

    void Awake() 
    {
        markers = new Dictionary<string, GameObject>();        
    }

    public void ClearMarkers()
    {
        foreach (GameObject go in markers.Values)
            Object.Destroy(go);
        markers.Clear();   
    }

}
