using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMarker : MonoBehaviour
{
    // Start is called before the first frame update
    public Material defaultMaterial;
    protected float _scale = 0;
    public readonly float kMaxScale = .5f;

    protected static AutoMat<Color> autoMat;

    void Awake()
    {
        if (autoMat == null)
            autoMat = new AutoMat<Color>();
    }

    void Start()
    {
        transform.localScale = new Vector3(0,0,0);
        _scale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (_scale < kMaxScale )
        {
            _scale =  Mathf.Clamp(_scale + Time.deltaTime*2,0,kMaxScale);
            transform.localScale = new Vector3(_scale,_scale,_scale);
        }
    }

    public void SetColor(Color newC)
    {
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();

        Material mat = autoMat.GetMaterial(newC);
        if (mat == null)
        {
            mat = new Material(defaultMaterial);
            mat.SetColor("_EmissionColor", newC);
            autoMat.AddMaterial(newC, mat);
        }
        foreach(Renderer renderer in renderers)
        {
            renderer.sharedMaterial = autoMat.GetMaterial(newC);
        }
    }
}
