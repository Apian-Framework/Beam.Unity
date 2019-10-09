using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeamBackend;

public class DemoBike : AIBike
{
    public DemoBike(string ID, Player p, Vector3 initialPos, Heading head) 
        : base(ID, p, initialPos, head)
    {

    }
}
