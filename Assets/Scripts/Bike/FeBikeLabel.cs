using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeBikeLabel : MonoBehaviour
{
    protected FrontendBike _feBike;
    protected GameObject _cameraGO;
    protected static AutoMat<Color> autoMat;

    void Awake()
    {
        if (autoMat == null)
            autoMat = new AutoMat<Color>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void UpdatePos()
    {
        if (_feBike != null)
        {
            Vector3 pos = _feBike.transform.position;
            pos.y += 2.0f; // TODO: make Y  a property

            Vector3 eyeVec = pos -  _cameraGO.transform.position;
            float dist = eyeVec.magnitude;

            float offAxisDeg = Mathf.Abs( Vector3.Angle(eyeVec, _cameraGO.transform.forward) );
            float offAxisMag = 1 + .5f * Mathf.Tan(offAxisDeg * Mathf.Deg2Rad); // approximate

            transform.position = pos;
            //transform.rotation = Quaternion.LookRotation(eyeVec, _cameraGO.transform.up);
            transform.rotation = _cameraGO.transform.rotation; // this works better

            // keep them the size they would be at 3m
            float sc =  Mathf.Max(dist, 3f) / 3f;
            sc *= 1/offAxisMag;
            sc *= .15f; // just make is smaller (could just use the text size property in the gameobject inspector)
            transform.localScale = new Vector3(sc, sc, sc);
        }
    }

    public void Setup(FrontendBike bike)
    {
        _feBike = bike;

        TextMesh tm = (TextMesh)transform.Find("LabelText").GetComponent<TextMesh>();
        tm.text = bike.bb.name;
        SetColor( utils.ColorFromName(bike.bb.team.Color));

        _cameraGO = BeamMain.GetInstance().gameCamera.gameObject;
    }

    public void SetColor(Color newC)
    {
        Renderer r = transform.Find("LabelText").GetComponent<Renderer>();
        Material mat = autoMat.GetMaterial(newC);
        if (mat == null)
        {
            mat = r.material;
            mat.color = newC;
            autoMat.AddMaterial(newC, mat);
        }
        r.sharedMaterial = autoMat.GetMaterial(newC);
    }

}
