using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Linq;
using UnityEngine;

namespace TSDC {
	
public static class TransformExtensions
{
	//! @url http://forum.unity3d.com/threads/12949-transform-Find-doesn-t-work
	public static Transform FindInChildren(this Transform go, string name)
    {
        return (from x in go.GetComponentsInChildren<Transform>()
                where x.gameObject.name == name
                select x.gameObject).First().transform;
    }
}
	
public static class TypeExtensions
{
	//! @url http://answers.unity3d.com/questions/400614/access-game-classes-from-within-editor-extension.html
	public static List< Type > GetAllDerivedClasses(this Type aBaseClass, string[] aExcludeAssemblies)
	{
	    List< Type > result = new List< Type >();
	    foreach (Assembly A in AppDomain.CurrentDomain.GetAssemblies())
	    {
	        bool exclude = false;
	        foreach (string S in aExcludeAssemblies)
	        {
	            if (A.GetName().FullName.StartsWith(S))
	            {
	                exclude = true;
	                break;
	            }
	        }
	        if (exclude)
	            continue;
	        if (aBaseClass.IsInterface)
	        {
	            foreach (Type C in A.GetExportedTypes())
	                foreach(Type I in C.GetInterfaces())
	                    if (aBaseClass == I)
	                    {
	                        result.Add(C);
	                        break;
	                    }
	        }
	        else
	        {
	            foreach (Type C in A.GetExportedTypes())
	                if (C.IsSubclassOf(aBaseClass))
	                    result.Add(C);
	        }
	    }
	    return result;
	}
	 
	//! @url http://answers.unity3d.com/questions/400614/access-game-classes-from-within-editor-extension.html
	public static List< Type > GetAllDerivedClasses(this Type aBaseClass)
	{
	    return GetAllDerivedClasses(aBaseClass, new string[0]);
	}
		
	//! @url http://stackoverflow.com/questions/2448800/given-a-type-instance-how-to-get-generic-type-name-in-c
	public static string ToGenericTypeString(this Type t)
    {
        if (!t.IsGenericType)
            return t.AssemblyQualifiedName;
        string genericTypeName = t.GetGenericTypeDefinition().AssemblyQualifiedName;
        genericTypeName = genericTypeName.Substring(0,
            genericTypeName.IndexOf('`'));
        string genericArgs = string.Join(",",
            t.GetGenericArguments()
                .Select(ta => ToGenericTypeString(ta)).ToArray());
        return genericTypeName + "<" + genericArgs + ">";
    }
}

public static class MonoBehaviourExtensions 
{
	public static T AddComponent<T>(this MonoBehaviour monoBehaviour) where T : UnityEngine.Component
	{
		return MonoBehaviourExtensions.AddComponent<T>(monoBehaviour, true);
	}

	public static T AddComponent<T>(this MonoBehaviour monoBehaviour, bool allowDuplicates) where T : UnityEngine.Component
	{
		T component = monoBehaviour.GetComponent<T>();
		if ((component == null) || ((component != null) && allowDuplicates))
		{
			component = monoBehaviour.gameObject.AddComponent<T>() as T;
		}
		return component;
	}

	public static void DestroyComponent(this MonoBehaviour monoBehaviour, Component component) 
	{
		UnityEngine.Object.DestroyImmediate(component);
	}
}

public static class StringExtensions
{
	public static string SplitCamelCase( this string str )
	{
		return Regex.Replace( Regex.Replace( str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2" ), @"(\p{Ll})(\P{Ll})", "$1 $2" );
	}
}
	
public static class ArrayExtensions
{
	public static T[] SubArray<T>(this T[] data, int index, int length)
	{
	    T[] result = new T[length];
	    Array.Copy(data, index, result, 0, length);
	    return result;
	}

	public static void Populate<T>(this T[] arr, T value )
	{
		for ( int i = 0; i < arr.Length;i++ ) 
		{
			arr[i] = value;
		}
	}
}

public static class ListExtensions 
{
    public static bool ContainsAll<T>( this List<T> thisList, List<T> otherList )
    {
        if( otherList.Count == 0 )
        {
            return false;
        }

        foreach( T t in otherList )
        {
            if( !thisList.Contains(t) )   
            {
                return false;
            }
        }
        return true;
    }
}

} // namespace TSDC

