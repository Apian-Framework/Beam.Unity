using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseBike
{
    public Player player {get; private set;} = null;
    public Vector2 position {get; private set;} = Vector2.zero; // always on the grid
    // NOTE: 2D position: x => east, y => north (in 3-space z is north and y is up)
    public Heading heading { get; private set;} = Heading.kNorth;

    public static readonly float speed =  15.0f;
    public static readonly float length = 2.0f;    

    public BaseBike()
    { 

    }

}
