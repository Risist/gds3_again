using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using audioEvent = AK.Wwise.Event;

public class DamageSoundEffects : MonoBehaviour
{
    HealthController healthController;

    public audioEvent onDamageEvent;
    public audioEvent onDeathEvent;

    void OnDamageSound(DamageData data)
    {
        onDamageEvent.Post(gameObject);
    }
    void OnDeathSound(DamageData data)
    {
        onDeathEvent.Post(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        healthController = gameObject.GetComponent<HealthController>();

        healthController.onDamageCallback += OnDamageSound;
        healthController.onDamageCallback += OnDeathSound;
    }

}
