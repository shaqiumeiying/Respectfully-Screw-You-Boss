using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseRestart : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Restart();
        }

        if (Input.GetKeyDown(KeyCode.JoystickButton3))
        {
            Restart();
        }

        void Restart()
        {
            SceneManager.LoadScene("Boss"); 
        }
    }
}
