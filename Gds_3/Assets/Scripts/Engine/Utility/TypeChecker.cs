using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class TypeChecker
{
    // if obj is not a type T sets reference to null and logs an error
    public static void ValidateType<T>( ref object obj)
    {
#if UNITY_EDITOR
        if (!(obj is T))
        {
            Debug.LogError(obj.ToString() + " is not a " + nameof(T));
            obj = null;
        }
#endif
    }

    // if obj is not an asset sets reference to null and logs an error
    public static void ValidateIsAsset(ref UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if (!UnityEditor.AssetDatabase.Contains(obj))
        {
            Debug.LogError(obj.ToString() + " is an asset");
            obj = null;
        }
#endif
    }

    // if obj is an asset sets reference to null and logs an error
    public static void ValidateNotAsset(ref UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        if (UnityEditor.AssetDatabase.Contains(obj))
        {
            Debug.LogError(obj.ToString() + " is an asset");
            obj = null;
        }
#endif
    }



}
