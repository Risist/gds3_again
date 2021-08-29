using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableObject
{
    static protected T _instance; 
    static public T Instance => GetInstance(
        autoCreateReference: true,
        logErrorIfNotResolved: true);


    protected static T GetInstance(bool autoCreateReference, bool logErrorIfNotResolved)
    {
        if (!_instance && autoCreateReference)
        {
            _instance = CreateInstance<T>();
        }

#if UNITY_EDITOR
        // if still we have no Instance
        if (!_instance && logErrorIfNotResolved)
        {
            Debug.LogError("Instance of type " + typeof(T).Name + " couldn't be resolved");
        }
#endif

        return _instance;
    }

    protected void Awake()
    {
        _instance = this as T;
    }
}
