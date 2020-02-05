
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scoreboard : MovableUICanvasItem
{
    public float otherYBase = -12;
    public float lineDy = -40;
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
        OtherPlayerLines.Sort((a, b) => b.GetComponent<ScoreboardLine>().prevScore.CompareTo(a.GetComponent<ScoreboardLine>().prevScore));

        float y = otherYBase;       
        foreach (GameObject otherLine in OtherPlayerLines)
        {
            RectTransform rt = otherLine.GetComponent<RectTransform>();
            Vector2 pos = rt.anchoredPosition;
            pos.y = y;
            rt.anchoredPosition = pos;
            y += lineDy;
        }
        onScreenPos.y = offScreenPos.y - OtherPlayerLines.Count * lineDy;  
        _isDirty = false;
    }

    public void SetDirty()
    {
        _isDirty = true;
    }

    public void SetLocalPlayerBike(GameObject localPlayerBike)
    {
        LocalPlayerLine.SendMessage("SetBike", localPlayerBike.transform.GetComponent("FrontendBike"));
    }

    public void AddBike(GameObject bike)
    {
        GameObject newLine = GameObject.Instantiate(LocalPlayerLine, transform);
        newLine.transform.SetParent(transform); // set it as a child of this
        newLine.SendMessage("SetBike", bike.transform.GetComponent("FrontendBike"));   
        OtherPlayerLines.Add(newLine);   
        _isDirty = true; // needs sorting
    }

    public void RemoveBike(GameObject bikeObj)
    {
        FrontendBike remBike = bikeObj.GetComponent<FrontendBike>();
        GameObject line = OtherPlayerLines.Find(l => (l.GetComponent<ScoreboardLine>()).bike == remBike);
        if (line != null) {
            OtherPlayerLines.Remove(line);
            _isDirty = true; // needs sorting     
            Object.Destroy(line);
        }
    }

}
