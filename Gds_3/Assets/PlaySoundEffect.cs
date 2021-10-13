using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioEvent = AK.Wwise.Event;


public class PlaySoundEffect : MonoBehaviour
{
    public AudioEvent[] audioEvent;
    
    void PlaySound(AudioEvent audioEvent, GameObject gameObject)
    {
        audioEvent.Post(gameObject);
    }

    void StopSound(AudioEvent audioEvent, GameObject gameObject)
    {
        audioEvent.Stop(gameObject, 1);
    }

    public void StartPlayer()
    {
        foreach(AudioEvent userAudioEvent in audioEvent)
        {
            PlaySound(userAudioEvent, this.gameObject);
        }
    }

    public void StopPlayer()
    {
        foreach (AudioEvent userAudioEvent in audioEvent)
        {
            StopSound(userAudioEvent, this.gameObject);
        }
    }
}
