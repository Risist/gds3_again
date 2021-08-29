using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTimeScript : MonoBehaviour
{
    [SerializeField] [Range(0.01f, 1.0f)] float timeScale = 1.0f;
    [SerializeField] bool isEnabled = true;

    private void Awake()
    {
        enabled = isEnabled;
    }
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = timeScale;
    }
}
