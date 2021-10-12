using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioEvent = AK.Wwise.Event;

public class MusicManager : MonoBehaviour
{
    public static bool enableMusic = true;
    
    public AudioEvent musicEvent;

    private static MusicManager _instance;
    private static bool isSoundPlaying = false;
    public static MusicManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (!isSoundPlaying)
        {
            PlayMusic();
        }
    }

    public void PlayMusic()
    {
        musicEvent.Post(gameObject);
        isSoundPlaying = true;
    }


    public void StopMusic()
    {
        musicEvent.Stop(gameObject, 1);
        isSoundPlaying = false;
    }
}
