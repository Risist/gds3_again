using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class IOSwitchableAnimation : MonoBehaviour
{
    public string animationNameOpen;
    public string animationNameClose;
    public int animationLayer = 0;
    public float eventActivationTime = 0.9f;
    [Required] public Animator animator;
    [Required] public IOAnimationState animationState;
    public UnityEvent onOpen;
    public UnityEvent onClose;


    int animationNameHashOpen;
    int animationNameHashClose;

    private void Start()
    {
        if (!animator)
            animator = GetComponent<Animator>();
        Debug.Assert(animator);

        animationNameHashOpen   = Animator.StringToHash(animationNameOpen);
        animationNameHashClose  = Animator.StringToHash(animationNameClose);
    }

    IEnumerator PlayAnimCoroutine(int hash, UnityEvent eventOnfinish, bool newOpenState)
    {
        animator.SetTrigger(hash);

        yield return null;

        while (animator.GetCurrentAnimatorStateInfo(animationLayer).normalizedTime < eventActivationTime)
        {
            yield return null;
        }

        eventOnfinish.Invoke();

        animationState.openState = newOpenState;
        animationState.playAnimCoroutine = null;
    }

    public void SwitchOpenState()
    {
        if (animationState.playAnimCoroutine == null)
        {
            if(animationState.openState)
            {
                animationState.playAnimCoroutine = StartCoroutine(PlayAnimCoroutine(animationNameHashClose,onClose, false));
            }else
            {
                animationState.playAnimCoroutine = StartCoroutine(PlayAnimCoroutine(animationNameHashOpen, onOpen, true));
            }
        }
    }


}
