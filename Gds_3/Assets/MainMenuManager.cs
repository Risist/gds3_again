using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    Vector3 cameraInitialPosition;
    public float shakeMagnetude = 0.05f, shakeTime = 0.5f;
    public Camera mainCamera;

    public float animationCdn = 1;
        [Header("Start ButtonAnimation")]
    public Animator animator;
    [SerializeField] private string startButtonAnimation = "StartBtnAnimation";
    [SerializeField] private string brakeButtonAnimation = "BrakeBtnAnimation";
    public UnityEvent action;

    //[Header("Other Button Animation")]
    //public Animator otnerBtnAnimator;
    //[SerializeField] private string otherButtonAnimation = "SettingsQuitBtn";
    //[SerializeField] private string closeButtonAnimation = "HideOtherBTNSAnimations";

    [Header("Exit Button Animations")]
    public Animator exitAnimator;
    [SerializeField] private string exitButtonAnimation = "ExitBtnAniamtion";
    [SerializeField] private string closeExitButtonAnimation = "CloseExitButtonAnimation";
    public UnityEvent exitActions;
    public UnityEvent exitCloseButtonAction;


    [Header("Credits Button Animations")]
    public Animator creditsAnimator;
    public UnityEvent credisActions;
    public UnityEvent credisCloseButtonActions;
    [SerializeField] private string openCreditsButtonAnimation = "OpenCreditsAnimation";
    [SerializeField] private string closeCreditsButtonAnimation = "CloseCreditsAnimation";

    [Header("Buttons")]
    public GameObject startButtonObject = null;
    public GameObject creditsButtonObject = null;
    public GameObject settingsButtonObject = null;
    public GameObject exitButtonObject = null;
    public GameObject exitYesBTN = null;
    public GameObject exitNOBTN = null;
    public GameObject closeCreditsButtonObject = null;


    void Start()
    {
        StartCoroutine(BtnAnimationStart());
        animator.Play(startButtonAnimation, 0, 0.0f);
       // otnerBtnAnimator.Play(otherButtonAnimation, 0, 0.0f);
    }
    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == startButtonObject)
            {
                Debug.Log("Button Clicked");
              //  otnerBtnAnimator.Play(closeButtonAnimation, 0, 0.0f);
                action.Invoke();
                animator.Play(brakeButtonAnimation, 0, 0.0f);
               // Destroy(startButtonObject, 1f);
            }

            //Button credits Clicked
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == creditsButtonObject)
            {
                Debug.Log("Button credits Clicked");
                creditsAnimator.Play(openCreditsButtonAnimation, 0, 0.0f);
                startButtonObject.SetActive(false);
              //  otnerBtnAnimator.Play(closeButtonAnimation, 0, 0.0f);
                credisActions.Invoke();

            }
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == closeCreditsButtonObject)
            {
                creditsAnimator.Play(closeCreditsButtonAnimation, 0, 0.0f);
                startButtonObject.SetActive(true);
                animator.Play(startButtonAnimation, 0, 0.0f);
               // otnerBtnAnimator.Play(otherButtonAnimation, 0, 0.0f);
                credisCloseButtonActions.Invoke();
            }

            //Button settings Clicked
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == settingsButtonObject)
            {
                Debug.Log("Button settings Clicked");
            }

            //Button Exit Clicked
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == exitButtonObject)
            {
                Debug.Log("Button exit Clicked");
               // otnerBtnAnimator.Play(closeButtonAnimation, 0, 0.0f);
                exitAnimator.Play(exitButtonAnimation, 0, 0.0f);
                exitActions.Invoke();
                startButtonObject.SetActive(false);
            }
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == exitYesBTN)
            {
                Debug.Log("Button exit Clicked");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
            }
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == exitNOBTN)
            {
                Debug.Log("Button exit Clicked");
                startButtonObject.SetActive(true);
                animator.Play(startButtonAnimation, 0, 0.0f);
             //   otnerBtnAnimator.Play(otherButtonAnimation, 0, 0.0f);
                exitAnimator.Play(closeExitButtonAnimation, 0, 0.0f);
                exitCloseButtonAction.Invoke();
            }
        }
              
    }
    IEnumerator BtnAnimationStart()
    {
        yield return new WaitForSeconds(animationCdn);
    }
    //Camera Shake
    public void ShakeCamera()
    {
        cameraInitialPosition = mainCamera.transform.position;
        InvokeRepeating("StartCameraShaking", 0f, 0.005f);
        Invoke("StopCameraShaking", shakeTime);
    }

    void StartCameraShaking()
    {
        float cameraShakingOffsetX = Random.value * shakeMagnetude * 2 - shakeMagnetude;
        float cameraShakingOffsetY = Random.value * shakeMagnetude * 2 - shakeMagnetude;
        Vector3 cameraIntermadiatePosition = mainCamera.transform.position;
        cameraIntermadiatePosition.x += cameraShakingOffsetX;
        cameraIntermadiatePosition.y += cameraShakingOffsetY;
        mainCamera.transform.position = cameraIntermadiatePosition;
    }

    void StopCameraShaking()
    {
        CancelInvoke("StartCameraShaking");
        mainCamera.transform.position = cameraInitialPosition;
    }
}
