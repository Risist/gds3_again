using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoSingleton<GameEvents>
{
    static public new GameEvents Instance => GetInstance(
        autoCreateReference: true,
        findReference: true,
        logErrorIfNotResolved: true);
    protected override EOverrideMode OverrideMode => EOverrideMode.EOnEnable;

    public event Action<GameObject> onDoorwayTrigger = (door) => { };
    public void DoorwayTrigger(GameObject door)
    {
        onDoorwayTrigger(door);
    }

    public event Action<GameObject> onHasDoorKey = (door) => { };

    public void HasDoorKey(GameObject door)
    {
        onHasDoorKey(door);
    }
    
    public event Action<GameObject> fadeInObject = (room) => { };

    public void CanFadeInObject(GameObject room)
    {
        fadeInObject(room);
    }
    public event Action<GameObject> fadeOutObject = (backRoom) => { };
    public void CanFadeOutObject(GameObject backRoom)
    {
        fadeOutObject(backRoom);
    }
}
