using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialTrigger : MonoBehaviour
{
    public Tutorial tutorial;
    public GameObject tutorialCanvas;
    Vector3 canvasTransform;
    public Animator animator;
    private bool panelIsOpen = false;
    private float timeScale = 1f;
    public UnityEvent action;
    [SerializeField] private string openAnimationName = "TutorialPanelAnimOpen";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" )
        {
            if (!panelIsOpen)
            {
                Time.timeScale = 0;
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                animator.Play(openAnimationName, 0, 0.0f);
                TriggerTutorial();
                Destroy(gameObject, 0.1f);
                panelIsOpen = true;
            }
        }
    }
    public void TriggerTutorial()
    {
        FindObjectOfType<TutorialManager>().StartTutorial(tutorial);
    }
    private void Update()
    {
        if (panelIsOpen)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                action.Invoke();
            }
        }
    }
}
