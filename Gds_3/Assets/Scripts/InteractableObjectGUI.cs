using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObjectGUI : MonoBehaviour
{
    [SerializeField] [TextArea] string textInfo = "";
    [SerializeField] GameObject objectToHighlight;

    Text textToChange; 
    cakeslice.Outline objectOutline;
    bool objectTriggered = false;

    void Start()
    {
        textToChange = GameObject.FindGameObjectWithTag("InteractiveObjectCanvas").GetComponentInChildren<Text>(true);
        if (objectToHighlight)
        {
            objectOutline = objectToHighlight.AddComponent<cakeslice.Outline>();
            if (objectOutline == null)
            {
                objectOutline = objectToHighlight.GetComponent<cakeslice.Outline>();
            }

            objectOutline.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (textToChange && CheckForPlayer(other.gameObject))
        {
            objectTriggered = true;
            textToChange.text = textInfo;
            textToChange.gameObject.SetActive(true);
            if (objectToHighlight)
            {
                objectOutline.enabled = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (objectTriggered)
        {
            textToChange.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (objectTriggered)
        {
            objectTriggered = false;
            textToChange.gameObject.SetActive(false);
            if (objectToHighlight)
            {
                objectOutline.enabled = false;
            }
        }
    }

    private bool CheckForPlayer(GameObject obj)
    {
        return obj.CompareTag("Player");
    }
}
