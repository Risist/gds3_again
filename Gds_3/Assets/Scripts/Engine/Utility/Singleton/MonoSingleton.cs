
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static protected T _instance;
    static public T Instance => GetInstance(
        autoCreateReference: false,
        findReference: true,
        logErrorIfNotResolved: true);

    static public T UnsafeInstance => GetInstance(
        autoCreateReference: false,
        findReference: true,
        logErrorIfNotResolved: false);

    // helper function used to specify allowed way OF initializing Instance
    // @findReference could Instance be found in hierarchy?
    // @findReference could Instance be created from scratch? 
    //      use this if there are no settable params 
    // @logErrorIfNotResolved if reference has been not found should error be logged?
    protected static T GetInstance(bool autoCreateReference, bool findReference, bool logErrorIfNotResolved)
    {
        if (!_instance && findReference)
        {
            _instance = FindObjectOfType<T>();
        }

        if (!_instance && autoCreateReference)
        {
            var obj = new GameObject(typeof(T).Name);
            _instance = obj.AddComponent<T>();
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
    

    protected enum EOverrideMode
    {
        EOnEnable,
        EAwake,
        ENone
    }
    // when the reference should be set
    protected virtual EOverrideMode OverrideMode => EOverrideMode.EOnEnable;


    protected void Awake()
    {
        if(OverrideMode == EOverrideMode.EAwake)
            _instance = this as T;
    }

    protected void OnEnable()
    {
        if (OverrideMode == EOverrideMode.EOnEnable)
            _instance = this as T;
    }
}