using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreboardLine : MonoBehaviour
{
    public FrontendBike bike = null;

    public int prevScore = -1;
    protected TextMeshProUGUI whoTextMesh = null;
    protected TextMeshProUGUI scoreTextMesh = null;
    // Start is called before the first frame update
    void Awake()
    {
        scoreTextMesh = transform.Find("Score").GetComponent<TextMeshProUGUI>(); 
        whoTextMesh = transform.Find("Who").GetComponent<TextMeshProUGUI>();       
    }

    // Update is called once per frame
    void Update()
    {
        if (bike.bb.score != prevScore)
        {
            transform.parent.GetComponent<Scoreboard>().SetDirty();
            prevScore = bike.bb.score;
            scoreTextMesh.text = prevScore.ToString();
        }
    }

    public void SetBike(FrontendBike b)
    {
        bike = b;
        whoTextMesh.text = (bike.isLocal ? "" : "*") + bike.bb.name;
        whoTextMesh.color = utils.hexToColor(b.bb.team.Color);
    }
}
