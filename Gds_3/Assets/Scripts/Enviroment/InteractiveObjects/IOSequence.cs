using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class IOSequence : MonoBehaviour
{
    private enum ESequenceOrder
    {
        EForward,
        ELooping,
        EPingpong,
    }

    [SerializeField] Timer timer;
    [SerializeField] ESequenceOrder sequenceOrder;
    [SerializeField] UnityEvent onSequenceProceed;
    [SerializeField] UnityEvent onSequenceFinish;
    [SerializeField] UnityEvent[] actionSequence;

    int sequenceId;
    int sequenceDirection = 1;

    public void RestartSequence()
    {
        sequenceId = 0;
        sequenceDirection = 1;
    }

    public void TryRun()
    {
        if (timer.IsReadyRestart())
        {
            actionSequence[sequenceId].Invoke();
            ProceedSequenceAction();
        }
    }

    private void ProceedSequenceAction()
    {
        sequenceId += sequenceDirection;
        onSequenceProceed.Invoke();

        if (sequenceId >= actionSequence.Length)
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

}
