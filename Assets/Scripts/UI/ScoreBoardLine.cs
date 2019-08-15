using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardLine : MonoBehaviour
{
    public Bike bike = null;

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
            transform.parent.GetComponent<ScoreBoard>().SetDirty();
            prevScore = bike.player.Score;
            scoreTextMesh.text = prevScore.ToString();
        }
    }

    public void SetBike(Bike b)
    {
        bike = b;
        whoTextMesh.text = bike.player.ScreenName;
        whoTextMesh.color = bike.player.Team.Color;
    }
}
