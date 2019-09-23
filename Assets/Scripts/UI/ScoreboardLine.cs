using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardLine : MonoBehaviour
{
    public FrontendBike bike = null;

    public int prevScore = -1;
    protected TextMeshPro whoTextMesh = null;
    protected TextMeshPro scoreTextMesh = null;
    // Start is called before the first frame update
    void Awake()
    {
        scoreTextMesh = transform.Find("Score").GetComponent<TextMeshPro>(); 
        whoTextMesh = transform.Find("Who").GetComponent<TextMeshPro>();       
    }

    // Update is called once per frame
    void Update()
    {
        if (bike.player.Score != prevScore)
        {
            transform.parent.GetComponent<Scoreboard>().SetDirty();
            prevScore = bike.player.Score;
            scoreTextMesh.text = prevScore.ToString();
        }
    }

    public void SetBike(FrontendBike b)
    {
        bike = b;
        whoTextMesh.text = bike.player.ScreenName;
        whoTextMesh.color = bike.player.Team.Color;
    }
}
