using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    private Image fadeImage;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        Color c = fadeImage.color;
        c.a = 0;
        fadeImage.color = c;
    }

    public IEnumerator FadeToWhiteAndLoad(string sceneName, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(0f, 1f, t / duration);

            fadeImage.color = new Color(1, 1, 1, a);
            yield return null;
        }

        Time.timeScale = 1f; // restore normal speed
        SceneManager.LoadScene(sceneName);
    }
}
