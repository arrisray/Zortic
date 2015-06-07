using System;

//! @ref http://wiki.unity3d.com/index.php?title=Singleton
// public sealed class Singleton<T> where T : class, new()
public class Singleton<T> where T : class, new()
{
    /// <summary>
    /// Singleton implementation, readonly and static ensure thread safeness.
    /// </summary>
    public static readonly T Instance = new T ();
}