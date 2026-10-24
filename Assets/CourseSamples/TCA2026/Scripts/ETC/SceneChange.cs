using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{

    public string SceneName;
    public bool _isSceneChange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _isSceneChange = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isSceneChange == true)
        {
            _isSceneChange = false;
            SceneManager.LoadScene(SceneName);
        }
    }
}
