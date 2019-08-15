using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MovableUISetItem
{
    public float otherYBase = .38f;
    public float lineDy = -.05f;
    public GameObject LocalPlayerLine = null;

    protected bool _isDirty = true;

    List<GameObject> OtherPlayerLines = null;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        LocalPlayerLine = GameObject.Find("LocalPlayerLine");
        OtherPlayerLines = new List<GameObject>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();   
        if (_isDirty)
            DoSort();
    }

    protected void DoSort()
    {
        // descending
        OtherPlayerLines.Sort((a, b) => b.GetComponent<ScoreBoardLine>().prevScore.CompareTo(a.GetComponent<ScoreBoardLine>().prevScore));

        float y = otherYBase;       
        foreach (GameObject otherLine in OtherPlayerLines)
        {
            Vector3 pos = otherLine.transform.localPosition;
            pos.y = y;
            otherLine.transform.localPosition = pos;
            y += lineDy;
        }
        _isDirty = false;
    }

    public void SetDirty()
    {
        _isDirty = true;
    }

    public void SetLocalPlayerBike(GameObject localPlayerBike)
    {
        LocalPlayerLine.SendMessage("SetBike", localPlayerBike.transform.GetComponent("Bike"));
    }

    public void AddBike(GameObject bike)
    {
        GameObject newLine = GameObject.Instantiate(LocalPlayerLine, transform); //.position, LocalPlayerLine.transform.rotation);
        //newLine.transform.parent = transform; // set it as a child of this
        newLine.SendMessage("SetBike", bike.transform.GetComponent("Bike"));   
        OtherPlayerLines.Add(newLine);   
        _isDirty = true; // needs sorting
    }

}
