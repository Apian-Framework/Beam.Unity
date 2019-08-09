using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BikeFactory : MonoBehaviour
{
    public GameObject bikePrefab;     

    //
    // API
    // 
    public static GameObject CreateDemoBike(Color bikeColor, Vector3 basePos = new Vector3())
	{
		Heading heading = PickRandomHeading();
		Vector3 pos = PositionForNewBike( heading, basePos, Ground.gridSize * 5 );
		return CreateDemoBike(bikeColor, pos,  heading);				
	}
    public GameObject CreateDemoBike(Color bikeColor, Vector3 pos, Heading heading)
	{ 
        return CreateBike(typeof(DemoBike), bikeColor, pos, heading);
    }

    public GameObject CreateAIBike(Color bikeColor, Vector3 basePos = new Vector3())
	{
		Heading heading = PickRandomHeading();
		Vector3 pos = PositionForNewBike( heading, basePos, Ground.gridSize * 5 );
		return CreateAIBike(bikeColor, pos,  heading);		
	}	
    public GameObject CreateAIBike(Color bikeColor,  Vector3 pos, Heading heading)
	{ 
        return CreateBike(typeof(AIBike), bikeColor, pos, heading);
    }

    public GameObject CreateLocalPlayerBike(Color bikeColor, Vector3 basePos = new Vector3())
	{
		Heading heading = PickRandomHeading();
		Vector3 pos = PositionForNewBike( heading, basePos, Ground.gridSize * 5 );
        return CreateLocalPlayerBike( bikeColor, pos, heading);
	}
    public GameObject CreateLocalPlayerBike(Color bikeColor, Vector3 pos, Heading heading)
    {    
        GameObject bike  = CreateBike(typeof(LocalPlayerBike), bikeColor, pos, heading);
        LocalPlayerBike pb = bike.transform.GetComponent<LocalPlayerBike>();    
        inputDispatch.localPlayerBike = pb;  
        return bike;
    }	

    //
    // Utility
    //

	// Bike Factory stuff

	static Heading PickRandomHeading() 
	{
		int headInt = (int)Mathf.Clamp( Mathf.Floor(Random.Range(0,(int)Heading.kCount)), 0, 3);
		Debug.Log(string.Format("Heading: {0}", headInt));
		return (Heading)headInt;
	}

	static  Vector3 PickRandomPos(Ground ground, Heading head, Vector3 basePos, float radius)
	{
		Vector3 p = Ground.NearestGridPoint(
					new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius)) + basePos );
		return p + GameConstants.unitOffsetForHeading[(int)head] * .5f * Ground.gridSize;
	}
	static Vector3 PositionForNewBike(Heading head, Vector3 basePos, float radius)
	{
		float minDist = Bike.length * 5;
		float closestD = -1;
		Vector3 newPos = Vector3.zero;
		while (closestD < minDist) 
		{
 			newPos = PickRandomPos( head, basePos,  radius);		
			closestD = _bikesList.Count == 0 ? minDist : _bikesList.Select( (bike) => Vector3.Distance(bike.transform.position, newPos)).Aggregate( (acc,next) => acc < next ? acc : next);
		}
		return newPos;
	}

    GameObject CreateBike(System.Type bikeType, Color bikeColor, Vector3 pos, Heading heading)
    {
        GameObject newBike = GameObject.Instantiate(bikePrefab, pos, Quaternion.identity) as GameObject;
		newBike.AddComponent(bikeType);
        newBike.transform.parent = ground.transform;
        Bike bk = (Bike)newBike.transform.GetComponent("Bike");
		bk.heading = heading;
        bk.SetColor(bikeColor);
		_bikesList.Add(newBike);
        return newBike;
    }    



}
