using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector : MonoBehaviour
{
    // Start is called before the first frame update
    protected float _scale = 0;
    public readonly float Scale = 1.0f;

    void Start()
    {
        // transform.localScale = new Vector3(0,0,0);
        // _scale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // if (_scale < kMaxScale )
        // {
        //     _scale =  Mathf.Clamp(_scale + Time.deltaTime*2,0,kMaxScale);
        //     transform.localScale = new Vector3(_scale,_scale,_scale);
        // }
    }

    public void SetColor(Color newC)
    {
        //transform.Find("Quad1").GetComponent<Renderer>().material.SetColor("_EmissionColor", newC);
        Component[] renderers = transform.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
            renderer.material.SetColor("_EmissionColor", newC);
    }
}
