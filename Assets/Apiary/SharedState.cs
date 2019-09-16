using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedState
{
    public float Time { get; private set; } = 0;
    public Dictionary<string, Player> Players { get; private set; } = null;

    public SharedState()
    {
        Players = new Dictionary<string, Player>();
    }

    // DoUpdate is called once per frame 
    // NOT A MONO CLASS!!!
    public void DoUpdate(float deltaSecs)
    {
        Time += deltaSecs;
        // Players do not update 
    }

    public bool AddPlayer(Player p)
    {
        if  ( Players.ContainsKey(p.ID))
            return false;
        
        Players[p.ID] =  p;
        return true;
    }

    public bool RemovePlayer(Player p)
    {
        return Players.Remove(p.ID);
    }

    public void ClearPlayers()
    {
        Players.Clear();
    }
}

