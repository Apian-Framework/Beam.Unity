using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeamBackend;

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

    public static GameObject CreateDemoBike( BaseBike bb,  Ground ground )
	{ 
        return CreateBike(typeof(DemoBike), bb, ground);
    }

    public static GameObject CreateAIBike( BaseBike bb,  Ground ground)
	{ 
        return CreateBike(typeof(AIBike), bb, ground);
    }

    public static GameObject CreateLocalPlayerBike( BaseBike bb,  Ground ground)
    {    
        GameObject bike  = CreateBike(typeof(LocalPlayerBike), bb, ground ); 
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
		return p + GameConstants.UnitOffset3ForHeading(head) * .5f * Ground.gridSize;
	}
	public static Vector3 PositionForNewBike(List<GameObject> otherBikes, Heading head, Vector3 basePos, float radius)
	{
		float minDist = BaseBike.length * 20; 
		float closestD = -1;
		Vector3 newPos = Vector3.zero;
		while (closestD < minDist) 
		{
 			newPos = PickRandomPos( head, basePos,  radius);		
			closestD = otherBikes.Count == 0 ? minDist : otherBikes.Select( (bike) => Vector3.Distance(bike.transform.position, newPos)).Aggregate( (acc,next) => acc < next ? acc : next);
		}
		return newPos;
	}

    static GameObject CreateBike(System.Type bikeType, BaseBike bb, Ground ground)
    {
        GameObject newBike = GameObject.Instantiate(BikeFactory.GetInstance().bikePrefab, bb.GetPos3(), Quaternion.identity) as GameObject;
		newBike.AddComponent(bikeType);
        newBike.transform.parent = ground.transform;
        FrontendBike bk = (FrontendBike)newBike.transform.GetComponent("FrontendBike");
		bk.Setup(bb);		
        return newBike;
    }    



}
