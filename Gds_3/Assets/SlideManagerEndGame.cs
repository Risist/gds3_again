using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SlideManagerEndGame : MonoBehaviour
{

    [SerializeField] private List<Transform> slides = default;
    [SerializeField] private Image fadeSlide = default;
    [Header("Config Values")]
    [SerializeField, Tooltip("The duration (in seconds) over which the fade slide will fade in / out")] private float fadeDuration = 0.75f;
    [SerializeField, Tooltip("All key codes that will move to the next slide if pressed")]
    private KeyCode[] nextSlideKeyCodes =
    {
        KeyCode.D,
        KeyCode.RightArrow,
        KeyCode.Space
    };
    [SerializeField, Tooltip("All key codes that will move to the previous slide if pressed")]
    private KeyCode[] previousSlideKeyCodes =
    {
        KeyCode.A,
        KeyCode.LeftArrow,
    };
    private int currentSlide = -1;
    private bool isTransitioning = false;
    public float fadeTime = 5;
    public float reapeteRate = 5;

    private void Start()
    {
        fadeSlide.color = Color.black;
        NextSlide();
        InvokeRepeating("NextSlide", fadeTime, reapeteRate);
    }

    public void Update()
    {
        ListenForInput();
    }

    private void ListenForInput()
    {
        if (isTransitioning)
        {
            return;
        }
        if (nextSlideKeyCodes.Any(Input.GetKeyDown))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        if (previousSlideKeyCodes.Any(Input.GetKeyDown))
        {
            PreviousSlide();
        }
    }

    private void NextSlide()
    {
        if (currentSlide == slides.Count - 1)
        {
            StartCoroutine(SlideTransition());
            isTransitioning = false;
            SceneManager.LoadScene("MainMenu");
            // return;
        }
        currentSlide++;
        StartCoroutine(SlideTransition());
    }

    private IEnumerator SlideTransition()
    {
        isTransitioning = true;
        yield return StartCoroutine(FadeToTargetColor(targetColor: Color.black));
        slides.ForEach(slide => slide.gameObject.SetActive(slides.IndexOf(slide) == currentSlide));
        yield return StartCoroutine(FadeToTargetColor(targetColor: Color.clear));
        isTransitioning = false;
    }

    private void PreviousSlide()
    {
        if (currentSlide == 0)
        {
            return;
        }
        currentSlide--;
        StartCoroutine(SlideTransition());
    }

    private IEnumerator FadeToTargetColor(Color targetColor)
    {
        float elapsedTime = 0.0f;
        Color startColor = fadeSlide.color;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeSlide.color = Color.Lerp(startColor, targetColor, elapsedTime / fadeDuration);
            yield return null;
        }
    }
}
