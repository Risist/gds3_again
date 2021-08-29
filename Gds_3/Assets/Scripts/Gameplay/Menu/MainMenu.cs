using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public Button buttonPrefab;
    public Transform buttonSpawnpoint;


    [System.Serializable]
    struct SceneData
    {
        [Scene]
        public string sceneName;
        public string buttonName;
    }

    [SerializeField] SceneData[] scenes;


    public void OpenScene(int id)
    {
        SceneManager.LoadScene(scenes[id].sceneName);
    }


    void SpawnSceneButtons()
    {
        for(int i = 0; i < scenes.Length; ++i)
        {
            var button = Instantiate(buttonPrefab, buttonSpawnpoint);
            
            // we cache the value of i in local scope because all lambdas will reference the same varriable - which is scene.Length
            var ii = i;
            button.onClick.AddListener(() => OpenScene(ii));
            var text = button.GetComponentInChildren<Text>();
            if(text)
            {
                text.text = scenes[i].buttonName;
            }
        }
    }

    private void Start()
    {
        SpawnSceneButtons();
    }

}
