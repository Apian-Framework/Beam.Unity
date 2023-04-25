using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSquare : MonoBehaviour
{
    public Material defaultMaterial;
    protected static AutoMat<Color> autoMat;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
