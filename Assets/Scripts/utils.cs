using UnityEngine;
using System.Collections.Generic;

public class utils
{
	static public GameObject SafeGameObject(MonoBehaviour mb)
	{
		// Turns out if you have a MonoBehavior-derived "thing" and say:
		//   GameObject go = thing?.gameObject;
		// you'll still get a null-object exception from Unity. You have to
		// explicitly test for null.
        return mb ? mb.gameObject : null;
	}
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

	public static Dictionary<string, Color32> ColorDict = new Dictionary<string, Color32> {
		{"yellow", new Color32(0xff, 0xff, 0x00, 0xff)},
		{"red",    new Color32(0xff, 0x00, 0x00, 0xff)},
		{"cyan",   new Color32(0x00, 0xff, 0xff, 0xff)},
		{"blue",   new Color32(0x30, 0x30, 0xff, 0xff)},
		{"magenta",new Color32(0xff, 0x00, 0xff, 0xff)}
	};



     public static Color ColorFromName(string name)
     {
        return ColorDict.ContainsKey(name) ? ColorDict[name] : ColorDict["magenta"];
     }

}
