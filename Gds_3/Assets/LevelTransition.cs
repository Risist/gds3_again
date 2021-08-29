using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelTransition : MonoBehaviour
{
    public UnityEvent action;

    private void Start()
    {
        action.Invoke();
    }
}
