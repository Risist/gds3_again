using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Camera Shake")]
    Vector3 cameraInitialPosition;
    public float shakeMagnetude = 0.05f, shakeTime = 0.5f;
    public Camera mainCamera;

    [Header("Start Buttons")]
    public GameObject _startButton = null;
    public Animator startButtonAnimator;
    [SerializeField] private string startButtonAnimation = "StartBtnAnimation";
    [SerializeField] private string brakeButtonAnimation = "BrakeBtnAnimation";
    public UnityEvent action;

    public Color clickStartButtonColor;


    [Header("Option Buttons")]
    public GameObject _optionsButton = null;
    public Color clickOptionButtonColor;

    [Header("Exit Buttons")]
    public GameObject _exitButton = null;
    public GameObject _agreeExit = null;
    public GameObject _disagreeExit = null;
    public GameObject _exitText = null;

    public Animator exitButtonAnimator;
    [SerializeField] private string exitButtonAnimation = "ExitBtnAniamtion";
    [SerializeField] private string closeExitButtonAnimation = "CloseExitButtonAnimation";
    public UnityEvent exitActions;
    public UnityEvent exitCloseButtonAction;
    public Color clickExitButtonColor;

    [Header("Buttons Color")]
    public TextMeshPro[] textMeshPros;
    public Color normalButtonColor;
    public Color exitButtonColor;

    void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit Hit;

        if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == _startButton)
        {
            textMeshPros[0].color = clickStartButtonColor;
        }
        else
        {
            textMeshPros[0].color = normalButtonColor;
        }
        if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == _optionsButton)
        {
            textMeshPros[1].color = clickOptionButtonColor;
        }
        else
        {
            textMeshPros[1].color = normalButtonColor;
        }

        if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == _exitButton)
        {
            textMeshPros[2].color = clickExitButtonColor;
        }
        else
        {
            textMeshPros[2].color = exitButtonColor;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == _startButton)
            {
                Debug.Log("Button Clicked");
                action.Invoke();
                startButtonAnimator.Play(brakeButtonAnimation, 0, 0.0f);
            }

            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == _exitButton)
            {
                startButtonAnimator.Play(exitButtonAnimation, 0, 0.0f);
                StartCoroutine(WaitForExitAnim());
                exitActions.Invoke();
                Debug.Log("Button exit Clicked");

            }
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == _agreeExit)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                        Application.Quit();
#endif
            }
            if (Physics.Raycast(ray, out Hit) && Hit.collider.gameObject == _disagreeExit)
            {
                startButtonAnimator.Play(closeExitButtonAnimation, 0, 0.0f);
                _agreeExit.SetActive(false);
                _disagreeExit.SetActive(false);
                _exitText.SetActive(false);
                exitCloseButtonAction.Invoke();
            }
        }
    }

    IEnumerator WaitForExitAnim()
    {
        yield return new WaitForSeconds(1.5f);
        _agreeExit.SetActive(true);
        _disagreeExit.SetActive(true);
        _exitText.SetActive(true);
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
