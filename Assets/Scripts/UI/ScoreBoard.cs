
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scoreboard : MovableUISetItem
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
        try {
            OtherPlayerLines.Sort((a, b) => b.GetComponent<ScoreboardLine>().prevScore.CompareTo(a.GetComponent<ScoreboardLine>().prevScore));

            float y = otherYBase;       
            foreach (GameObject otherLine in OtherPlayerLines)
            {
                Vector3 pos = otherLine.transform.localPosition;
                pos.y = y;
                otherLine.transform.localPosition = pos;
                y += lineDy;
            }
        } catch(System.NullReferenceException e) {}
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
        onScreenPos.y = offScreenPos.y - OtherPlayerLines.Count * lineDy * transform.localScale.y;
    }

    // public void RemoveBike(GameObject bikeObj)
    // {
    //     Bike remBike = bikeObj.GetComponent<Bike>();
    //     GameObject line = OtherPlayerLines.Find(l => (l.GetComponent<ScoreboardLine>()).bike == remBike);
    //     OtherPlayerLines.Remove(line);
    //     Object.Destroy(line);
    // }

}
