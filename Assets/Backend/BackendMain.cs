using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendMain
{
    public BackendEvents EventPub {get; private set;} = null;
    public float Time { get; private set; } = 0;
    public Dictionary<string, Player> Players { get; private set; } = null;

    public Dictionary<string, BaseBike> Bikes { get; private set; } = null;

    public BackendMain()
    {
        EventPub = new BackendEvents();
        Players = new Dictionary<string, Player>();
        Bikes = new Dictionary<string, BaseBike>();        
    }

    // DoUpdate is called once per frame 
    // NOT A MONO CLASS!!!
    public void DoUpdate(float deltaSecs)
    {
        // TODO: get rid of this "long first frame" guard
        deltaSecs = Mathf.Min(deltaSecs, .125f);
        Time += deltaSecs;
        // Players do not update 
        foreach( BaseBike bb in Bikes.Values)
            bb.DoUpdate(deltaSecs);
    }

    //
    // Player
    //
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

    //
    // Bikes
    //
    public bool AddBike(BaseBike bb) {
        if  ( Bikes.ContainsKey(bb.bikeId))
            return false;
        
        Bikes[bb.bikeId] =  bb;
        return true;
    }

    public bool RemoveBike(BaseBike bb)
    {
        return Bikes.Remove(bb.bikeId);
    }

    public void ClearBikes()
    {
        Bikes.Clear();
    }    
}

