using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeamBackend
{
    public interface IFrontendModeHelper 
    {
        void OnStartMode(int modeId, object param);
        void DispatchCmd(int modeId, int cmdId, object param);
        void OnEndMode(int modeId, object param);
    }


    public interface IBeamFrontend 
    {

        // Called by backend

        // Game Modes
        IFrontendModeHelper ModeHelper();

        // Players
        void OnNewPlayer(Player p);
        // Bikes
        
        void OnNewBike(IBike ib);
        void OnBikeDestroyed(string bikeId, bool doExplode);
        void OnClearBikes();
        void OnBikeAtPlace(string bikeId, Ground.Place place);

        // Places
        void OnFreePlace(Ground.Place p);     
        void OnClearPlaces();
    }

}
