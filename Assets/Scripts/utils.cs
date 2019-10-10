using UnityEngine;
using System.Collections;

public class utils 
{
    public static Vector3 Vec3(Vector2 v2, float y=0) => new Vector3(v2.x, y, v2.y);	

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

     public static Color hexToColor(string hex)
     {
         hex = hex.Replace ("0x", "");//in case the string is formatted 0xFFFFFF
         hex = hex.Replace ("#", "");//in case the string is formatted #FFFFFF
         byte a = 255;//assume fully visible unless specified in hex
         byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
         byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
         byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
         //Only use alpha if the string has enough characters
         if(hex.Length == 8){
             a = byte.Parse(hex.Substring(6,2), System.Globalization.NumberStyles.HexNumber);
         }
         return new Color32(r,g,b,a);
     }	
	
}
