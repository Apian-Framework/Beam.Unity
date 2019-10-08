using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeamBackend
{
    public interface IFrontendProxy {

        // Players

        // Bikes
        public void NewBike(BaseBike bb);
        public void DestroyBike(string bikeId, bool doExplode);
        public void DestroyBikes();
        public void ReportBikeAtPoint(string bikeId, Vector2 pos);

        // Places
        public void SetupPlaceMarker(Ground.Place p);
        public void FreePlaceMarker(Ground.Place p);     
        public void ClearPlaceMarkers();

    }

}
