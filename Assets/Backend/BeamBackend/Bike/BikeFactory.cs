using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BeamBackend;

public static class BikeFactory
{
  
    public static IBike CreateDemoBike(string ID, Player p, Vector3 initialPos, Heading head)
	{ 
        IBike ib = new DemoBike(ID, p, initialPos, head);
        return ib;
    }

    public static IBike CreateAIBike( string ID, Player p, Vector3 initialPos, Heading head)
	{ 
        IBike ib = new AIBike(ID, p, initialPos, head);
        return ib;
    }

    // public static IBike CreateLocalPlayerBike( BaseBike bb,  FeGround ground)
    // {    
    //     GameObject bike  = CreateBike(typeof(LocalPlayerBike), bb, ground ); 
    //     return bike;
    // }	

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
		Vector3 p = FeGround.NearestGridPoint(
					new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius)) + basePos );
		return p + GameConstants.UnitOffset3ForHeading(head) * .5f * Ground.gridSize;
	}
	public static Vector2 PositionForNewBike(List<IBike> otherBikes, Heading head, Vector3 basePos, float radius)
	{
		float minDist = BaseBike.length * 20; 
		float closestD = -1;
		Vector2 newPos = Vector2.zero;
		while (closestD < minDist) 
		{
 			newPos = PickRandomPos( head, basePos,  radius);		
			closestD = otherBikes.Count == 0 ? minDist : otherBikes.Select( (bike) => Vector2.Distance(bike.position, newPos)).Aggregate( (acc,next) => acc < next ? acc : next);
		}
		return newPos;
	}

    // static GameObject CreateBike(System.Type bikeType, BaseBike bb, FeGround ground)
    // {
    //     GameObject newBike = GameObject.Instantiate(FeBikeFactory.GetInstance().bikePrefab, bb.GetPos3(), Quaternion.identity) as GameObject;
	// 	newBike.AddComponent(bikeType);
    //     newBike.transform.parent = ground.transform;
    //     FrontendBike bk = (FrontendBike)newBike.transform.GetComponent("FrontendBike");
	// 	bk.Setup(bb);		
    //     return newBike;
    // }    



}
