using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class IOPlayAnimation : MonoBehaviour
{
    public string animationName;
    public int animationLayer = 0;
    public float eventActivationTime = 0.9f;
    [Required] public Animator animator;
    public UnityEvent onAnimationFinish;

    
    int animationNameHash;
    bool animationStarted;

    Coroutine playAnimCoroutine;

    private void Start()
    {
        if(!animator)
            animator = GetComponent<Animator>();
        Debug.Assert(animator);
        
        animationNameHash = Animator.StringToHash(animationName);
    }

    IEnumerator PlayAnimCoroutine()
    {
        animator.SetTrigger(animationNameHash);

        yield return null;

        while (animator.GetCurrentAnimatorStateInfo(animationLayer).normalizedTime < eventActivationTime)
        {
            yield return null;
        }

        onAnimationFinish.Invoke();

        playAnimCoroutine = null;
    }

    public void PlayAnim()
    {
        if (playAnimCoroutine == null)
        {
            playAnimCoroutine = StartCoroutine(PlayAnimCoroutine());
        }
    }



}
