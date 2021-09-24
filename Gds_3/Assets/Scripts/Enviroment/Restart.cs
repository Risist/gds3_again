using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [SerializeField] GameObject restrtPanel = null;
    public Animator restartButtonAnimator;
    [SerializeField] private string restartButtonAnimation = "DeathMenuRestartButtonAnimationON";
    [SerializeField] private string restartExitButtonAnimation = "DeathMenuRestartButtonAnimationOFF";
    public Animator exitButtonAnimator;
    [SerializeField] private string exitButtonAnimation = "DeathMenuExitButtonAnimationON";
    [SerializeField] private string exittExitButtonAnimation = "DeathMenuExitButtonAnimationOFF";
     
    private void Start()
    {
        var health = GetComponentInParent<HealthController>();
        health.onDeathCallback += (DamageData data) =>
        {
            if (restrtPanel)
            {
                restrtPanel.gameObject.SetActive(true);
                restartButtonAnimator.Play(restartButtonAnimation, 0, 0.0f);
                exitButtonAnimator.Play(exitButtonAnimation, 0, 0.0f);
            }
        };
    }
}


