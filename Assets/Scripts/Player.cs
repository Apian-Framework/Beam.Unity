using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: Monbehaviour here? Probably not needed but makes it easier
// to see what they are doing while the game is running

public static class SplashPlayers
{
    public static readonly int count = 12;
    public static readonly Player[] data = {
        new Player("00", "Alice", Team.teamData[(int)TeamID.kSharks]),    
        new Player("01", "Bob", Team.teamData[(int)TeamID.kCatfish]),  
        new Player("02", "Carol", Team.teamData[(int)TeamID.kWhales]),  
        new Player("03", "Don", Team.teamData[(int)TeamID.kOrcas]), 
        new Player("04", "Evan", Team.teamData[(int)TeamID.kSharks]),    
        new Player("05", "Frank", Team.teamData[(int)TeamID.kCatfish]),  
        new Player("06", "Gayle", Team.teamData[(int)TeamID.kWhales]),  
        new Player("07", "Herb", Team.teamData[(int)TeamID.kOrcas]),  
        new Player("08", "Inez", Team.teamData[(int)TeamID.kSharks]),    
        new Player("09", "Jim", Team.teamData[(int)TeamID.kCatfish]),  
        new Player("0a", "Kayla", Team.teamData[(int)TeamID.kWhales]),  
        new Player("0b", "Mike", Team.teamData[(int)TeamID.kOrcas]),                        
    };
}

public class Player
{
    public string ID { get; private set;}    
    public string ScreenName { get; private set;}
    public Team Team {get; private set;}
    public int Score { get; set;}

    public Player(string id, string name, Team t)
    { 
        ID = id;
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
