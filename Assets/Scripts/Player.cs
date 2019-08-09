using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: Monbehaviour here? Probably not needed but makes it easier
// to see what they are doing while the game is running

public static class DemoPlayers
{
    public static readonly int count = 8;
    public static readonly Player[] data = {
        new Player("Alice", Team.teamData[(int)TeamID.kSharks]),    
        new Player("Bob", Team.teamData[(int)TeamID.kCatfish]),  
        new Player("Carol", Team.teamData[(int)TeamID.kWhales]),  
        new Player("Don", Team.teamData[(int)TeamID.kOrcas]), 
        new Player("Evan", Team.teamData[(int)TeamID.kSharks]),    
        new Player("Frank", Team.teamData[(int)TeamID.kCatfish]),  
        new Player("Gayle", Team.teamData[(int)TeamID.kWhales]),  
        new Player("Herb", Team.teamData[(int)TeamID.kOrcas]),                
    };
}

public class Player : MonoBehaviour
{
    public string ScreenName { get; private set;}
    public Team Team {get; private set;}
    public int Score { get; private set;}

    public Player(string name, Team t)
    { 
        ScreenName = name;
        Team = t;
        Score = 0;
    }

    // void Start()
    // {

    // }

    // Update is called once per frame
    // void Update()
    // {
        
    // }
}
