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

    public void PlayFootstepSound()
    {
        footstepEvent.Post(gameObject);
    }
    public void PlayAttackSound()
    {
        attackEvent.Post(gameObject);
    }

    public void PlaySpecialAttackSound1()
    {
        attackSpecial1Event.Post(gameObject);
    }

    public void PlaySpecialAttackSound2()
    {
        attackSpecial2Event.Post(gameObject);
    }

    public void PlayDashSound()
    {
        dashEvent.Post(gameObject);
    }

    public void PlayBodyfallSound()
    {
        deathBodyfallEvent.Post(gameObject);
    }
}
