using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoSingleton<CheckpointManager>
{
    CheckpointSet[] checkpointSet;
    string lastSceneName;

    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        // Check if scene has changed
        if(SceneManager.GetActiveScene().name != lastSceneName)
        {
            lastSceneName = SceneManager.GetActiveScene().name;
            checkpointSet = FindObjectsOfType<CheckpointSet>();
        }
    }


}
