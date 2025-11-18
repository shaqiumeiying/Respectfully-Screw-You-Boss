using UnityEngine;
using System.Collections;

public class SlideUI : MonoBehaviour
{
    public RectTransform target;
    public Vector2 hiddenPos;
    public Vector2 shownPos;
    public float duration = 0.5f;

    public void SlideIn()
    {
        StopAllCoroutines();
        StartCoroutine(Slide(hiddenPos, shownPos));
    }

    public void SlideOut(System.Action onComplete = null)
    {
        StopAllCoroutines();
        StartCoroutine(Slide(shownPos, hiddenPos, onComplete));
    }

    IEnumerator Slide(Vector2 from, Vector2 to, System.Action onComplete = null)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            target.anchoredPosition = Vector2.Lerp(from, to, t / duration);
            yield return null;
        }

        target.anchoredPosition = to;
        onComplete?.Invoke();
    }
}
