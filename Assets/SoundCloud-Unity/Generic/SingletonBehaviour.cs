using System;
using UnityEngine;

namespace SoundCloud
{

/// <summary>
/// Attribute to define a Singleton that persists across scenes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public class Persistent : Attribute
{
}

/// <summary>
/// Singleton generic base class for classes derived from MonoBehaviour.
/// </summary>
/// <typeparam name="T">Instance class.</typeparam>
[DisallowMultipleComponent]
public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance = null;
    private static object instanceLock = new object();
    private static bool applicationIsQuitting = false;

    protected void Awake()
    {
        lock (instanceLock)
        {
            if (instance != null && instance != this)
            {
                Debug.Log(typeof(T).ToString() + " destroyed. Another instance already exists.");
                Destroy(this.gameObject);
            }
            else
            {
                instance = this as T;

                if (Attribute.IsDefined(typeof(T), typeof(Persistent)))
                {
                    DontDestroyOnLoad(instance.gameObject);
                }
            }
        }

        AwakeSingleton();
    }

    protected virtual void AwakeSingleton() { }

    protected void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

    public static T Instance
    {
        get
        {
            if (applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed. Not creating another.");
                return null;
            }

            lock (instanceLock)
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<T>();

                    if (FindObjectsOfType<T>().Length > 1)
                    {
                        Debug.LogError("[Singleton] Multiple Singleton instances!");
                        return instance;
                    }

                    if (instance == null)
                    {
                        GameObject go = new GameObject("(singleton) " + typeof(T).ToString());
                        instance = go.AddComponent<T>();

                        Debug.Log("[Singleton] Created instance of " + typeof(T) + ".");
                    }
                    else
                    {
                        Debug.Log("[Singleton] Using instance already created of " + typeof(T) + ".");
                    }

                    if (Attribute.IsDefined(typeof(T), typeof(Persistent)))
                    {
                        DontDestroyOnLoad(instance.gameObject);
                    }
                }

                return instance;
            }
        }
    }
}

}
