using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using BarkSystem;
using NaughtyAttributes;

public class IOBark : MonoBehaviour
{
    private enum ESequenceOrder
    {
        EForward,
        ELooping,
        EPingpong,
    }

    [SerializeField] bool playOnPlayer;

    [Space]
    [SerializeField] bool useSequence;

    [SerializeField, ShowIf("useSequence")] ESequenceOrder sequenceOrder;
    [SerializeField, HideIf("useSequence")] bool oneOff;

    [SerializeField, ShowIf("CanShowTimer")] Timer timer = new Timer(0);

    [SerializeField, ShowIf("useSequence")] UnityEvent onSequenceProceed;
    [SerializeField, ShowIf("useSequence")] UnityEvent onSequenceFinish;
    [SerializeField, ShowIf("useSequence")] BarkSet[] barkSequence;

    [SerializeField, HideIf("useSequence")] UnityEvent onBarkPlayed;
    [SerializeField, HideIf("useSequence"), Expandable] BarkSet barkSet;


    



    bool CanShowTimer => useSequence || !oneOff;

    bool alreadyPlayed;
    int sequenceId;
    int sequenceDirection = 1;

    Transform player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        RestartSequence();
    }

    public void RestartSequence()
    {
        sequenceId = 0;
        sequenceDirection = 1;
    }

    private void ProceedSequenceAction()
    {
        sequenceId+= sequenceDirection;
        onSequenceProceed.Invoke();

        if (sequenceId >= barkSequence.Length)
        {
            FinishSequenceAction();
        }
    }

    private void FinishSequenceAction()
    {
        switch (sequenceOrder)
        {
            case ESequenceOrder.EForward:
                break;
            case ESequenceOrder.ELooping:
                sequenceId = 0;
                break;
            case ESequenceOrder.EPingpong:
                sequenceId *= -1;
                break;
        }
        onSequenceFinish.Invoke();
    }

    public void PlayBark()
    {
        if(useSequence)
        {
            if (sequenceId >= barkSequence.Length)
                return;

            if (timer.IsReady())
            {
                
                bool barkSucceded = BarkManager.Instance.CallBark(playOnPlayer ? player : transform, barkSequence[sequenceId]);
                if ( barkSucceded)
                {
                    alreadyPlayed = true;
                    timer.Restart();
                    ProceedSequenceAction();
                }
            }
        }
        else
        {
            if (oneOff && alreadyPlayed)
                return;

            if (timer.IsReady())
            {
                bool barkSucceded = BarkManager.Instance.CallBark(playOnPlayer ? player : transform, barkSet);
                if(barkSucceded)
                {
                    alreadyPlayed = true;
                    timer.Restart();
                    onBarkPlayed.Invoke();
                }
            }
        }

        
    }
}
