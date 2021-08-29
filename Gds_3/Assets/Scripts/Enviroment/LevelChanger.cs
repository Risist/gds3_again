using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelChanger : MonoBehaviour
{
    #region FIELDS
    public Image fadeOutUIImage = null;
    public float fadeSpeed = 0.8f;
    private float timeScale = 1f;
    public enum FadeDirection
    {
        In, //Alpha = 1
        Out // Alpha = 0
    }
    #endregion
    #region MONOBHEAVIOR
    void OnEnable()
    {
        StartCoroutine(Fade(FadeDirection.Out));
    }
    #endregion

    #region FADE
    private IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = (fadeDirection == FadeDirection.Out) ? 1 : 0;
        float fadeEndValue = (fadeDirection == FadeDirection.Out) ? 0 : 1;
        if (fadeDirection == FadeDirection.Out)
        {
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
            fadeOutUIImage.enabled = false;
        }
        else
        {
            fadeOutUIImage.enabled = true;
            while (alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);
                yield return null;
            }
        }
    }
    #endregion
    public void FadeToNextLevel()
    {
        StartCoroutine(FadeAndLoadScene(FadeDirection.In));
    }

    #region HELPERS
    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection)
    {
        yield return Fade(fadeDirection);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        fadeOutUIImage.color = new Color(fadeOutUIImage.color.r, fadeOutUIImage.color.g, fadeOutUIImage.color.b, alpha);
        alpha += Time.deltaTime * (1.0f / fadeSpeed) * ((fadeDirection == FadeDirection.Out) ? -1 : 1);
    }
    #endregion
}
