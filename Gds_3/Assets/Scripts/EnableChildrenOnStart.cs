using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableChildrenOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
