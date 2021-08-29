using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class DeactivateAfterDelay : MonoBehaviour
{
    public enum EMode
    {
        EDeactivate,
        EDestroy,
    }
    public Timer tReady;
    public EMode mode;

    private void OnEnable()
    {
        tReady.Restart();
    }
    private void Update()
    {
        if(tReady.IsReady())
        {
            if(mode == EMode.EDeactivate)
            {
                gameObject.SetActive(false);
            }
            else if(mode == EMode.EDestroy)
            {
                Destroy(gameObject);
            }
        }
    }
}
