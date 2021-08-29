using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class MinimalTimer
{
    public MinimalTimer()
    {
        actualTime = 0;
    }

    // actual time of timmer; when the timmer was last restarted
    [System.NonSerialized]
    public float actualTime = 0;

    // resets actual time
    public void Restart()
    {
        actualTime = Time.time;
    }
    public void Restart(float diff)
    {
        actualTime = Time.time + diff;
    }
    public float ElapsedTime()
    {
        return Time.time - actualTime;
    }

    // returns true if time elapsed from last reset is greather than passed argument (cd)
    public bool IsReady(float cd)
    {
        return Time.time - actualTime >= cd;
    }
    // the same as above but automatically resets if timer was ready
    public bool IsReadyRestart(float cd)
    {
        if (Time.time - actualTime >= cd)
        {
            Restart();
            return true;
        }
        return false;
    }
}

/*
 * usage:
 * create timmer, set up cd;
 * 
 * if(timmer.isReadyRestart())
 *      do something every [cd] time (in seconds)
 */

[System.Serializable]
public class Timer : MinimalTimer
{

    public Timer(float _cd)
    {
        actualTime = 0;
        cd = _cd;
    }
    public Timer()
    {
        actualTime = 0;
    }
    // how much time have to be elapsed from last reset to be ready
    public float cd = 1;


    public void RestartRandom(float cdMin, float cdMax)
    {
        actualTime = Time.time;
        cd = UnityEngine.Random.Range(cdMin, cdMax);
    }
    public void RestartRandom(RangedFloat cdRange)
    {
        actualTime = Time.time;
        cd = UnityEngine.Random.Range(cdRange.min, cdRange.max);
    }


    // returns true if time elapsed from last reset is greather than public member of this class (cd)
    public bool IsReady()
    {
        return IsReady(cd);
    }
    new public bool IsReady(float cd)
    {
        return base.IsReady(cd);
    }
    // the same as above but automatically resets if timer was ready
    public bool IsReadyRestart()
    {
        return IsReadyRestart(cd);
    }
    new public bool IsReadyRestart(float cd)
    {
        return base.IsReadyRestart(cd);
    }
}



/*
[System.Serializable]
public class RandomTimer : MinimalTimer
{
    public RangedFloat cdRange;
    public RandomTimer(float cdMin, float cdMax)
    {
        actualTime = 0;
        cdRange = new RangedFloat(cdMin, cdMax);
        cd = UnityEngine.Random.Range(cdRange.min, cdRange.max);
    }
    public RandomTimer()
    {
        actualTime = 0;
    }
    // how much time have to be elapsed from last reset to be ready
    public float cd = 1;

    // returns true if time elapsed from last reset is greather than public member of this class (cd)
    public bool IsReady()
    {
        return IsReady(cd);
    }
    new public bool IsReady(float cd)
    {
        return base.IsReady(cd);
    }
    // the same as above but automatically resets if timer was ready
    public bool IsReadyRestart()
    {
        return IsReadyRestart(cd);
    }
    new public bool IsReadyRestart(float cd)
    {
        return base.IsReadyRestart(cd);
    }
}*/
