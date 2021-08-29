using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject restartPanel = null;
    private float timeScale = 1f;


    private void Start()
    {
        mainPanel.SetActive(false);
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = timeScale;
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = timeScale;
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
        Time.timeScale = timeScale;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && mainPanel.activeInHierarchy == false)
        {
            mainPanel.SetActive(true);
            Time.timeScale = 0;

        }
        else if (Input.GetKeyDown(KeyCode.Escape) && mainPanel.activeInHierarchy == true)
        {
            Time.timeScale = timeScale;
            mainPanel.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (mainPanel.activeInHierarchy)
        {
            restartPanel.SetActive(false);
        }
    }
}
 