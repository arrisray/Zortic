using UnityEngine;

//! @ref http://zingweb.com/blog/2012/04/26/unity-singletons/
public abstract class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static T _instance = null;
    private static bool m_isQuitting = false;

    public bool isQuitting { get { return m_isQuitting; } }

    /// <summary>
    /// gets the instance of this Singleton
    /// use this for all instance calls:
    /// MyClass.Instance.MyMethod();
    /// or make your public methods static
    /// and have them use Instance
    /// </summary>
    public static T Instance {
      get {
          if (m_isQuitting)
          {
            // /// Debug.LogWarning( "Application is quitting." );
            return _instance;
          }

          if (_instance == null) {
              // /// Debug.LogWarning( typeof(T) + " singleton instance has not been created yet." );
              _instance = (T)FindObjectOfType (typeof(T));
              if (_instance == null) {
                  // /// Debug.LogWarning( "Unable to find " + typeof(T) + " singleton Object." );

                  string goName = typeof(T).ToString ();           
                  
                  GameObject go = GameObject.Find (goName);
                  if (go == null) {
                      // /// Debug.LogWarning( "Unable to find " + typeof(T) + " singleton GameObject -- creating it now." );
                      go = new GameObject ();
                      go.name = goName;
                  }
                  
                  _instance = go.AddComponent<T> ();                 
                  // /// Debug.LogWarning( "Adding " + typeof(T) + " Component." );
              }
          }
          return _instance;
      }
    }

	public virtual void OnEnable()
    {
    }

    /// <summary>
    /// for garbage collection
    /// </summary>
    // public virtual void OnApplicationQuit ()
    public virtual void OnDisable()
    {
        // release reference on exit
		Destroy( this );
        _instance = null;
    }

    public virtual void OnApplicationQuit()
    {
        // /// Debug.Log( this.name + "::OnApplicationQuit" );
        m_isQuitting = true;
    }

	public virtual void Reset()
	{
	}

    // in your child class you can implement Awake()
    // and add any initialization code you want such as
    // DontDestroyOnLoad(go);
    // if you want this to persist across loads
    // or if you want to set a parent object with SetParent()

    /// <summary>
    /// parent this to another gameobject by string
    /// call from Awake if you so desire
    /// </summary>
    protected void SetParent (string parentGOName)
    {
      if (parentGOName != null) {
          GameObject parentGO = GameObject.Find (parentGOName);
          if (parentGO == null) {
              parentGO = new GameObject ();
              parentGO.name = parentGOName;
          }
          this.transform.parent = parentGO.transform;
      }
    }
}