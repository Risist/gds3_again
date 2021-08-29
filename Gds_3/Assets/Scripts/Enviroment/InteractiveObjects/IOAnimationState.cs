using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;

public class IOAnimationState : MonoBehaviour
{
    public Coroutine playAnimCoroutine;
    [ReadOnly] public bool openState;
}
