using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeamBackend
{
    public interface IFrontendProxy {

        // Players
        void NewPlayer(Player p);
        // Bikes
        void NewBike(IBike ib);
        void DestroyBike(string bikeId, bool doExplode);
        void DestroyBikes();
        void ReportBikeAtPoint(string bikeId, Vector2 pos);

        // Places
        void SetupPlaceMarker(Ground.Place p);
        void FreePlaceMarker(Ground.Place p);     
        void ClearPlaceMarkers();

    }

}
