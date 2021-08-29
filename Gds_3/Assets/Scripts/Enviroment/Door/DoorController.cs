using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private Animator doorAnim = null;
    private bool doorOpen = false;
    [SerializeField] private string openAnimationName = "DoorOpen";
    [SerializeField] private string closeAnimationName = "DoorClose";
    [SerializeField] private int waitTimer = 1;
    [SerializeField] private bool pauseInterection = false;
    [SerializeField] private KeyItemController _keyItem = null;
    [SerializeField] private GameObject keyCounterPanel = null;
    // [SerializeField] private GameObject door;


    private void Start()
    {
        GameEvents.Instance.onDoorwayTrigger += OnDoorwayOpen;
        GameEvents.Instance.onHasDoorKey += OnHasDoorKey;
    }
    public void OnDoorwayOpen(GameObject door)
    {
        if (door)
        {
            if (!doorOpen && !pauseInterection)
            {
                doorAnim.Play(openAnimationName, 0, 0.0f);
                doorOpen = true;
                StartCoroutine(PauseDoorInterection());
            }
            else if (doorOpen && !pauseInterection)
            {
                //doorAnim.Play(closeAnimationName, 0, 0.0f);
                doorOpen = true;
                StartCoroutine(PauseDoorInterection());
            }
        }
    }
    public void OnHasDoorKey(GameObject door)
    {
        if (door)
        {
            if (_keyItem.hasKey == true)
            {
                keyCounterPanel.SetActive(false);
                if (!doorOpen && !pauseInterection)
                {
                    doorAnim.Play(openAnimationName, 0, 0.0f);
                    doorOpen = true;
                    StartCoroutine(PauseDoorInterection());
                }
                else if (doorOpen && !pauseInterection)
                {
                    //doorAnim.Play(closeAnimationName, 0, 0.0f);
                    doorOpen = true;
                    StartCoroutine(PauseDoorInterection());
                }
            }
        }
    }       

    private IEnumerator PauseDoorInterection()
    {
        pauseInterection = true;
        yield return new WaitForSeconds(waitTimer);
        pauseInterection = false;
    }
}
