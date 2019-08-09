using UnityEngine;

public enum TeamID {
    kSharks = 0,
    kCatfish = 1,
    kWhales = 2,
    kOrcas = 3
}

public class Team
{
    public static readonly Team[] teamData = new Team[] {
        new Team("Sharks", Color.yellow),
        new Team("Catfish", Color.red),
        new Team("Whales", Color.cyan),
        new Team("Orcas", Color.blue)                        
    };

    public string Name;
    public Color Color;

    public Team(string name, Color col) 
    {
        Name = name;
        Color = col;
    }
}