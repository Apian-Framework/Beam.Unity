using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeamBackend
{
    public interface IBeamFrontend {

        // From backend
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
