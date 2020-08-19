using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeamGameCode;

public class Connector : MonoBehaviour
{
    public Material defaultMaterial;
    protected float _scale = 0;
    public readonly float Scale = 1.0f;

    public string fromTo;
   protected static AutoMat<Color> autoMat;

    void Awake()
    {
        if (autoMat == null)
            autoMat = new AutoMat<Color>();
    }

    void Start()
    {
        fromTo = "(free)";
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

    public void SetupForPlaces(BeamPlace p1, BeamPlace p2)
    {
        transform.position = (utils.Vec3(p1.GetPos()) + utils.Vec3(p2.GetPos())) * .5f;
        fromTo = $"({p1.PosHash}, {p2.PosHash})";
        Vector3 angles = transform.eulerAngles;
        angles.z = 0;
        angles.y = p1.xIdx == p2.xIdx ? 0 : 90;
        transform.eulerAngles = angles;

		SetColor(utils.hexToColor(p1.bike.team.Color));
    }

    public void SetColor(Color newC)
    {
        Material newMat = new Material(defaultMaterial);
        newMat.SetColor("_EmissionColor", newC);
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        foreach(Renderer renderer in renderers)
            renderer.sharedMaterial = autoMat.GetMaterial(newC, newMat);
    }
}
