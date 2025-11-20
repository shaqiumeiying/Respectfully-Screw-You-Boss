using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonAction : MonoBehaviour
{
    public string sceneToLoad;

    public void LoadScene()
    {
        Debug.Log("Loading scene: " + sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }
}
