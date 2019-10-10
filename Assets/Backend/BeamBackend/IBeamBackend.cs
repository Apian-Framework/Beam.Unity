using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BeamBackend
{
    public interface IBeamBackend {

        void OnTurnRequested(string bikeId, TurnDir turn);
    }

}
