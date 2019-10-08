using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class FrontendProxy : IFrontendProxy
{
    //
    // API
    // 

    // Players

    // Bikes
    public void NewBike(BaseBike bb)
    {

    }
    public void DestroyBike(string bikeId, bool doExplode)
    {

    }  
    public void DestroyBikes()
    {
        throw(new Exception("Not Implmented"));
    }    

    public void ReportBikeAtPoint(string bikeId, Vector2 pos)
    {
        OnBikeAtPoint(new BikeAtPointArgs(bikeId, pos));
    }    

    // Ground
    public void SetupPlaceMarker(Ground.Place p)
    {
        throw(new Exception("Not Implmented"));            
    }
    public void FreePlaceMarker(Ground.Place p)
    {
        throw(new Exception("Not Implmented"));            
    }        
    public void ClearPlaceMarkers()
    {
        throw(new Exception("Not Implmented"));
    }



    //
    // Events
    //

    public class BikeAtPointArgs
    {
        public string bikeId {get; private set;}
        public Vector2 pos {get; private set;}
        public BikeAtPointArgs(string bike, Vector2 p) { bikeId = bike; pos = p; }
    }
    public event EventHandler<BikeAtPointArgs> BikeAtPoint;
    protected virtual void OnBikeAtPoint(BikeAtPointArgs args) =>  BikeAtPoint?.Invoke(this, args);
}  

