using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    [SerializeField] GameObject mainPanel = null;
    private void Start()
    {
        var health = GetComponentInParent<HealthController>();
        health.onDeathCallback += (DamageData data) =>
        {
            if (mainPanel)
            {
                mainPanel.gameObject.SetActive(true);
            }
        };
    }
}


