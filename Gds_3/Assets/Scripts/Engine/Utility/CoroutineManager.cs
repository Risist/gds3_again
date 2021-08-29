using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/*
 * Place to run coroutines that wont be randomly turned off by object activation state
 * Also provides some utility functions in context of corroutines
 */
public class CoroutineManager : MonoSingleton<CoroutineManager>
{
    static public new CoroutineManager Instance => GetInstance(
        autoCreateReference: true,
        findReference: true,
        logErrorIfNotResolved: true);

    // waits for s seconds
    public void StartDelay(Action a, float s)
    {
        StartCoroutine(Delay(a, s));
    }
    public static IEnumerator Delay(Action a, float s)
    {
        yield return new WaitForSeconds(s);
        a();
    }
    public void StartDelayFrame(Action a)
    {
        StartCoroutine(DelayFrame(a));
    }
    public static IEnumerator DelayFrame(Action a)
    {
        yield return new WaitForEndOfFrame();
        a();
    }

    public void StartWaitFor(Action a, Func<bool> condition)
    {
        StartCoroutine(WaitFor(a, condition));
    }
    public static IEnumerator WaitFor(Action a, Func<bool> condition)
    {
        yield return new WaitUntil(condition);
        a();
    }

    public void StartWaitForFrame(Action a, Func<bool> condition)
    {
        StartCoroutine(WaitForFrame(a, condition));
    }
    public static IEnumerator WaitForFrame(Action a, Func<bool> condition)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(condition);
        a();
    }

}
