using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BikeFactory : MonoBehaviour
{
    public GameObject bikePrefab;     

    // Singleton management
    private static BikeFactory instance = null;
    public static BikeFactory GetInstance()
    {
        if (instance == null)
        {
            instance = (BikeFactory)GameObject.FindObjectOfType(typeof(BikeFactory));
            if (!instance)
                Debug.LogError("There needs to be one active BikeFactory script on a GameObject in your scene.");
        }
 
        return instance;
    }    

    //
    // API
    // 

    public static GameObject CreateDemoBike( Player p, Ground ground, Vector3 pos, Heading heading)
	{ 
        return CreateBike(typeof(DemoBike), p, ground, pos, heading);
    }

    public static GameObject CreateAIBike( Player p, Ground ground, Vector3 pos, Heading heading)
	{ 
        return CreateBike(typeof(AIBike), p, ground, pos, heading);
    }

    public static GameObject CreateLocalPlayerBike(Player p, Ground ground, Vector3 pos, Heading heading)
    {    
        GameObject bike  = CreateBike(typeof(LocalPlayerBike), p, ground, pos, heading); 
        return bike;
    }	

    //
    // Utility
    //

	// Bike Factory stuff

	public static Heading PickRandomHeading() 
	{
		int headInt = (int)Mathf.Clamp( Mathf.Floor(Random.Range(0,(int)Heading.kCount)), 0, 3);
		// Debug.Log(string.Format("Heading: {0}", headInt));
		return (Heading)headInt;
	}

	static  Vector3 PickRandomPos( Heading head, Vector3 basePos, float radius)
	{
		Vector3 p = Ground.NearestGridPoint(
					new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius)) + basePos );
		return p + GameConstants.unitOffsetForHeading[(int)head] * .5f * Ground.gridSize;
	}
	public static Vector3 PositionForNewBike(List<GameObject> otherBikes, Heading head, Vector3 basePos, float radius)
	{
		float minDist = Bike.length * 20; 
		float closestD = -1;
		Vector3 newPos = Vector3.zero;
		while (closestD < minDist) 
		{
 			newPos = PickRandomPos( head, basePos,  radius);		
			closestD = otherBikes.Count == 0 ? minDist : otherBikes.Select( (bike) => Vector3.Distance(bike.transform.position, newPos)).Aggregate( (acc,next) => acc < next ? acc : next);
		}
		return newPos;
	}

    static GameObject CreateBike(System.Type bikeType, Player p, Ground ground, Vector3 pos, Heading heading)
    {
        GameObject newBike = GameObject.Instantiate(BikeFactory.GetInstance().bikePrefab, pos, Quaternion.identity) as GameObject;
		newBike.AddComponent(bikeType);
        newBike.transform.parent = ground.transform;
        Bike bk = (Bike)newBike.transform.GetComponent("Bike");
		bk.Setup(heading, p);		
        return newBike;
    }    



}
