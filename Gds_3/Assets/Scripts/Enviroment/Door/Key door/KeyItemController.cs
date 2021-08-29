using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class KeyItemController : MonoBehaviour
{
    public bool hasKey = false;
    public int keys;
    public TextMeshProUGUI counterText;
  

    private void FixedUpdate()
    {
        counterText.text = "" + keys;
        if (keys == 2)
        {
           hasKey = true;
        }
    }
}