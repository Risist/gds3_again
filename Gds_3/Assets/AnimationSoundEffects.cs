using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using audioEvent = AK.Wwise.Event;

public class AnimationSoundEffects : MonoBehaviour
{
    public audioEvent footstepEvent;
    public audioEvent attackEvent;
    public audioEvent attackSpecial1Event;
    public audioEvent attackSpecial2Event;
    public audioEvent dashEvent;
    public audioEvent deathBodyfallEvent;

    public bool ignoreFirstPlay = false;

    public void PlayFootstepSound()
    {
        if (!ignoreFirstPlay)
        {
            footstepEvent.Post(gameObject);
        }
        else
        {
            ignoreFirstPlay = false;
        }
    }
    public void PlayAttackSound()
    {
        
        if (!ignoreFirstPlay)
        {
            attackEvent.Post(gameObject);
        }
        else
        {
            ignoreFirstPlay = false;
        }
    }

    public void PlaySpecialAttackSound1()
    {
        
        if (!ignoreFirstPlay)
        {
            attackSpecial1Event.Post(gameObject);
        }
        else
        {
            ignoreFirstPlay = false;
        }
    }

    public void PlaySpecialAttackSound2()
    {
        
        if (!ignoreFirstPlay)
        {
            attackSpecial2Event.Post(gameObject);
        }
        else
        {
            ignoreFirstPlay = false;
        }
    }

    public void PlayDashSound()
    {
        
        if (!ignoreFirstPlay)
        {
            dashEvent.Post(gameObject);
        }
        else
        {
            ignoreFirstPlay = false;
        }
    }

    public void PlayBodyfallSound()
    {
        
        if (!ignoreFirstPlay)
        {
            deathBodyfallEvent.Post(gameObject);
        }
        else
        {
            ignoreFirstPlay = false;
        }
    }
}
