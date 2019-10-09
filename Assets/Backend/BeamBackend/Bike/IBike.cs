using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BeamBackend
{
    public interface IBike 
    {
        string bikeId {get;}   
        Player player {get;}   
        Vector2 position {get;}   
        Heading heading { get;}   
        void Loop(float secs);                              
    }

}
