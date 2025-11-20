//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//using System.Collections;

//public class FadeController : MonoBehaviour
//{
//    private Image fadeImage;

//    void Awake()
//    {
//        fadeImage = GetComponent<Image>();
//        Color c = fadeImage.color;
//        c.a = 0;
//        fadeImage.color = c;
//    }

//    public IEnumerator FadeToWhiteAndLoad(string sceneName, float duration)
//    {
//        float t = 0f;

//        while (t < duration)
//        {
//            t += Time.unscaledDeltaTime;
//            float a = Mathf.Lerp(0f, 1f, t / duration);

//            fadeImage.color = new Color(1, 1, 1, a);
//            yield return null;
//        }

//        Time.timeScale = 1f; // restore normal speed
//        SceneManager.LoadScene(sceneName);
//    }
//}
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
        c.a = 0f;
        fadeImage.color = c;
    }

    public IEnumerator Fade(Color startColor, Color endColor, float duration, string sceneToLoad = null)
    {
        float t = 0f;

        fadeImage.color = startColor;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float lerp = Mathf.Clamp01(t / duration);

            fadeImage.color = Color.Lerp(startColor, endColor, lerp);
            yield return null;
        }

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(sceneToLoad);
        }
    }
    public IEnumerator FadeAlpha(float fromA, float toA, float duration, string sceneToLoad = null)
    {
        Color start = fadeImage.color;
        start.a = fromA;

        Color end = fadeImage.color;
        end.a = toA;

        return Fade(start, end, duration, sceneToLoad);
    }
}
