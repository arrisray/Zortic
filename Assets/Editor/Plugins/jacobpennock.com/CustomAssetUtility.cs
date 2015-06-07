using UnityEngine;
using UnityEditor;
using System.IO;

public static class CustomAssetUtility
{
    //! @note Disable Unity > Preferences > General > Verify Saving Assets to avoid annoying save dialogs everytime a new asset is created.
	
	public static T CreateAsset<T>() where T : ScriptableObject
	{
		return CustomAssetUtility.CreateAsset<T>( string.Empty );
	}
	
    public static T CreateAsset<T>( string assetPath ) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T> ();
		
        if( assetPath == string.Empty ) 
        {
			assetPath = ( Selection.activeObject != null ) ? AssetDatabase.GetAssetPath ( Selection.activeObject ) : "Assets";
        } 
		/*
        else if( Path.GetExtension(path) != "" ) 
        {
            path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
        }
        */
        
		assetPath += "/New " + typeof(T).ToString() + ".asset";
        assetPath = AssetDatabase.GenerateUniqueAssetPath( assetPath );
        
        AssetDatabase.CreateAsset( asset, assetPath );
        AssetDatabase.SaveAssets ();
		
        // EditorUtility.FocusProjectWindow ();
        // Selection.activeObject = asset;
        return asset;
    }

    public static void DestroyAsset<T>( T asset ) where T : ScriptableObject
    {
        AssetDatabase.DeleteAsset( AssetDatabase.GetAssetPath( asset ) );
    }
}
