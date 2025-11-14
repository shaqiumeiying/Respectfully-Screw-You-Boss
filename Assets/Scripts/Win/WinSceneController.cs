using System.Collections;
using UnityEngine;

public class WinSceneController : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer whiteFade;
    public Transform player;
    public Transform playerTargetPosition;
    public SpriteRenderer textImage;

    [Header("Timings")]
    public float fadeDuration = 1f;
    public float moveDuration = 1.2f;
    public float textFadeDuration = 1f;

    void Start()
    {
        StartCoroutine(Sequence());
    }

    IEnumerator Sequence()
    {
        // Start fully white
        SetAlpha(whiteFade, 1f);
        SetAlpha(textImage, 0f);

        // 1. Fade white ¡ú transparent
        yield return FadeSprite(whiteFade, 1f, 0f, fadeDuration);

        // 2. Move player
        Vector3 startPos = player.position;
        Vector3 endPos = playerTargetPosition.position;
        float t = 0f;

        while (t < moveDuration)
        {
            t += Time.deltaTime;
            float lerp = t / moveDuration;
            player.position = Vector3.Lerp(startPos, endPos, lerp);
            yield return null;
        }

        // 3. Fade text in
        yield return FadeSprite(textImage, 0f, 1f, textFadeDuration);
    }

    // Helper: instantly set alpha
    void SetAlpha(SpriteRenderer sr, float a)
    {
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }

    // Helper: fade sprite renderer
    IEnumerator FadeSprite(SpriteRenderer sr, float from, float to, float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = t / duration;

            Color c = sr.color;
            c.a = Mathf.Lerp(from, to, lerp);
            sr.color = c;

            yield return null;
        }
    }
}
