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
    public void NewPlayer(Player p)
    {
        UnityEngine.Debug.Log("** Need to implement FEP.NewPlayer()");
    }

    // Bikes
    public void NewBike(IBike ib)
    {
        UnityEngine.Debug.Log("** Need to implement FEP.NewBike()");
    }
    public void DestroyBike(string bikeId, bool doExplode)
    {
        UnityEngine.Debug.Log("** Need to implement FEP.DestroyBike()");
    }  
    public void DestroyBikes()
    {
        UnityEngine.Debug.Log("** Need to implement FEP.DestroyBikes()");
    }    

    public void ReportBikeAtPoint(string bikeId, Vector2 pos)
    {
        UnityEngine.Debug.Log(string.Format("** Reporting bike {0} at {1}", bikeId, pos));        
        OnBikeAtPoint(new BikeAtPointArgs(bikeId, pos));
    }    

    // Ground
    public void SetupPlaceMarker(Ground.Place p)
    {
        UnityEngine.Debug.Log("** Need to implement FEP.SetupPlaceMarker()");     
    }
    public void FreePlaceMarker(Ground.Place p)
    {
        UnityEngine.Debug.Log("** Need to implement FEP.FreePlaceMarker()");           
    }        
    public void ClearPlaceMarkers()
    {
        UnityEngine.Debug.Log("** Need to implement FEP.ClearPlaceMarkers()"); 
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

