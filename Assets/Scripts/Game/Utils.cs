using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;

public class Utils
{	
	// --------------------------------------------------------------------------
    // ENUMS

    public enum Direction
    {
    	Unknown = 0,
    	Left = 1<<0,
    	Right = 1<<1,
    	Forward = 1<<2,
    	Reverse = 1<<3,
    }

	// --------------------------------------------------------------------------
    // METHODS

    public static void Assert(bool condition)
	{
	    if (!condition) 
    	{
    		throw new System.Exception();
    	}
	}
	
	public static string UppercaseFirst(string s)
    {
		// Check for empty string.
		if (string.IsNullOrEmpty(s))
		{
		    return string.Empty;
		}
		// Return char and concat substring.
		return char.ToUpper(s[0]) + s.Substring(1);
    }
	
	//! @url http://stackoverflow.com/questions/9854900/instantiate-an-class-from-its-string-name
	public static IEnumerable<Type> GetDerivedTypesFor<T>() // Type baseType)
	{
		Type baseType = typeof(T);
	    var assembly = Assembly.Load ( "Assembly-CSharp" ); // Assembly.GetExecutingAssembly();
	
	    return assembly.GetTypes()
	        .Where(baseType.IsAssignableFrom)
	        .Where(t => baseType != t);
	}
		
	//! @url http://stackoverflow.com/questions/5411694/get-all-inherited-classes-of-an-abstract-class
	public static class ReflectiveEnumerator
	{
	    static ReflectiveEnumerator() { }
	
	    public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class//, IComparable<T>
	    {
	        List<T> objects = new List<T>();
	        foreach (Type type in 
	            Assembly.GetAssembly(typeof(T)).GetTypes()
	            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
	        {
	            objects.Add((T)Activator.CreateInstance(type, constructorArgs));
	        }
	        // objects.Sort();
	        return objects;
	    }
	}
	
	//! @url http://stackoverflow.com/questions/4738280/use-reflection-to-call-generic-method-on-object-instance-with-signature-someobj
	static public System.Object InvokeGenericMethod( Type instanceType, string methodName, Type typeName, System.Object value )
    {
        // Just for simplicity, assume it's public etc
        MethodInfo method = instanceType.GetMethod( methodName );
        MethodInfo generic = method.MakeGenericMethod( typeName );
		
		System.Object[] args = ( value != null ) ? new System.Object[] { value } : null;
        return generic.Invoke( null, args );
	}

    static public Vector3 CalculateTorque(Rigidbody rigidbody, Vector3 oldPoint, Vector3 newPoint)
    {
        Vector3 x = Vector3.Cross(oldPoint.normalized, newPoint.normalized);
        float theta = Mathf.Asin(x.magnitude);
        Vector3 w = x.normalized * theta / Time.fixedDeltaTime;

        Quaternion q = rigidbody.rotation * rigidbody.inertiaTensorRotation;
        Vector3 T = q * Vector3.Scale(rigidbody.inertiaTensor, (Quaternion.Inverse(q) * w));
        return T;
    }

    //! @brief Find the relative direction of b with regard to a.
    //! @ref Unity Developer Network, "Left/Right Test Function", @HigherScriptingAuthority, http://forum.unity3d.com/threads/31420-Left-Right-test-function
    static public Direction FindRelativeDirection( Vector3 a, Vector3 b, Vector3 up ) 
    {
		Vector3 cross = Vector3.Cross(a, b);
		float dot = Vector3.Dot(cross, up);
		
		if (dot > 0.0) 
		{
			// /// Debug.Log( "a=" + a + ", b=" + b + ", dot=" + dot + ", dir=Right");
			return Direction.Right; 
		} 
		else if (dot < 0.0) 
		{
			// /// Debug.Log( "a=" + a + ", b=" + b + ", dot=" + dot + ", dir=Left");
			return Direction.Left; 
		} 
		else 
		{
			dot = Vector3.Dot(a, b);
			Direction d = (dot >= 0.0) ? Direction.Forward : Direction.Reverse;
			// /// Debug.Log( "a=" + a + ", b=" + b + ", dot=" + dot + ", dir=" + d);
			return d;
		}
	}

	public static Vector2 ToVector2( string rString )
	{
		if( rString.Length < 3)
		{
			return new Vector2();
		}
		
	    string[] temp = rString.Substring(1,rString.Length-2).Split(',');
	    float x = float.Parse(temp[0]);
	    float y = float.Parse(temp[1]);
	    Vector2 rValue = new Vector2(x,y);
	    return rValue;
	}

	public static Vector3 ToVector3( string rString )
	{
		if( rString.Length < 3)
		{
			return new Vector3();
		}

	    string[] temp = rString.Substring(1,rString.Length-2).Split(',');
	    float x = float.Parse(temp[0]);
	    float y = float.Parse(temp[1]);
	    float z = float.Parse(temp[2]);
	    Vector3 rValue = new Vector3(x,y,z);
	    return rValue;
	}
	
	//! @url http://answers.unity3d.com/questions/246116/how-can-i-generate-a-guid-or-a-uuid-from-within-un.html
	public static string GetUniqueID()
	{
		string key = "ID";
		
		var random = new System.Random();              
		DateTime epochStart = new System.DateTime(1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
		
		string uniqueID = 
			Application.systemLanguage                 //Language
			+"-"+ Application.platform                           //Device   
			+"-"+String.Format("{0:X}", Convert.ToInt32(timestamp))          //Time
			+"-"+String.Format("{0:X}", Convert.ToInt32(Time.time*1000000))     //Time in game
			+"-"+String.Format("{0:X}", random.Next(1000000000));          //random number
		
		// /// Debug.Log("Generated Unique ID: "+uniqueID);
		
		if( PlayerPrefs.HasKey(key) )
		{
			uniqueID = PlayerPrefs.GetString(key);      
		} 
		else 
		{       
			PlayerPrefs.SetString(key, uniqueID);
			PlayerPrefs.Save();  
		}
		
		return uniqueID;
    }

	// C#
	//! @url http://answers.unity3d.com/questions/179775/game-window-size-from-editor-window-in-editor-mode.html
	public static Vector2 GetMainGameViewSize()
	{
	    System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
	    System.Reflection.MethodInfo GetSizeOfMainGameView = T.GetMethod("GetSizeOfMainGameView",System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
	    System.Object Res = GetSizeOfMainGameView.Invoke(null,null);
	    return (Vector2)Res;
	}
}
