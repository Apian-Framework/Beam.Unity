using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackendEvents 
{
    public void ReportBikeAtPoint(string bikeId, Vector2 pos)
    {
        OnBikeAtPoint(new BikeAtPointArgs(bikeId, pos));
    }

    public class BikeAtPointArgs
    {
        public string bikeId {get; private set;}
        public Vector2 pos {get; private set;}
        public BikeAtPointArgs(string bike, Vector2 p) { bikeId = bike; pos = p; }
    }
    public event EventHandler<BikeAtPointArgs> BikeAtPoint;
    protected virtual void OnBikeAtPoint(BikeAtPointArgs args) =>  BikeAtPoint?.Invoke(this, args);

    
}
