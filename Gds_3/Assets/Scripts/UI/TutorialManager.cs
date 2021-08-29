using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    private Queue<string> sentences;
    public TextMeshProUGUI text = null;
    public RawImage videoPlayer;
    public UnityEvent action;
    public Animator animator;
    public GameObject gameObject;
    [SerializeField] private string closeAnimationName = "TutorialPanelAnimClose";

    private float timeScale = 1f;
    void Start()
    {
        sentences = new Queue<string>();
    }
   public void StartTutorial(Tutorial tutorial)
    {
        videoPlayer.GetComponent<RawImage>().texture = tutorial.videoClips.texture;
        sentences.Clear();

        foreach(string sentence in tutorial.setences)
        {
            sentences.Enqueue(sentence);
        }
        DisplayNextTutorial();
    }   
    public void DisplayNextTutorial()
    {
        if(sentences.Count == 0)
        {
            EndTutorial();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    IEnumerator TypeSentence(string sentence)
    {
        text.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            text.text += letter;
            yield return null;      
        }
    }
    public void EndTutorial()
    {
        animator.Play(closeAnimationName, 0, 0.0f);
        Time.timeScale = timeScale;
    }
}
