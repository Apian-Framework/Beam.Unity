using UnityEngine;
using System.Collections;

public class utils 
{

	static public Vector3 MeshObjSize(GameObject theObj)
	{
		MeshFilter mf = (MeshFilter)theObj.GetComponent<MeshFilter>();
		Transform tf = (Transform)theObj.GetComponent<Transform>();
		return Vector3.Scale(mf.sharedMesh.bounds.size, tf.localScale);		
	}
	
	static public Vector3 objectPosByName(string objName)
	{
		Vector3 pos = Vector3.zero;
		
		GameObject obj = GameObject.Find(objName);
		if (obj)
		{
			pos = obj.transform.position;
		}
		
		return pos;
	}
	
	static public Component findObjectComponent(string objName, string compTypeName)
	{
		Component comp = null;
		GameObject obj = null;
		
		obj = GameObject.Find(objName);
		if (obj!= null)
			comp = obj.GetComponent(compTypeName);
		
		return comp;
	}
 
    public static Vector2 stringPair2Vector2(string s) // assumes "x,y"
    {
        string[] vals = s.Split(',');   
        
        return new Vector2( float.Parse(vals[0]), float.Parse(vals[1]));
    }    
	
}
