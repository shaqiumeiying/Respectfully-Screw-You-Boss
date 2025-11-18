using UnityEngine;
using UnityEngine.EventSystems;

public class HoverPopLose : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform target;

    [Header("Pop Settings")]
    public float popScale = 1.1f;
    public float popHeight = 20f;   // how much it lifts upward
    public float animTime = 0.12f;

    private Vector3 originalScale;
    private Vector2 originalPos;
    private bool isPopped = false;

    void Start()
    {
        if (!target) target = GetComponent<RectTransform>();

        originalScale = target.localScale;
        originalPos = target.anchoredPosition;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Pop();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Unpop();
    }

    public void Pop()
    {
        if (isPopped) return;
        isPopped = true;

        StopAllCoroutines();
        StartCoroutine(Animate(target.localScale, originalScale * popScale,
                               target.anchoredPosition, originalPos + new Vector2(0, popHeight)));
    }

    public void Unpop()
    {
        if (!isPopped) return;
        isPopped = false;

        StopAllCoroutines();
        StartCoroutine(Animate(target.localScale, originalScale,
                               target.anchoredPosition, originalPos));
    }

    private System.Collections.IEnumerator Animate(Vector3 fromScale, Vector3 toScale,
                                                   Vector2 fromPos, Vector2 toPos)
    {
        float t = 0f;

        while (t < animTime)
        {
            t += Time.deltaTime;
            float blend = t / animTime;

            target.localScale = Vector3.Lerp(fromScale, toScale, blend);
            target.anchoredPosition = Vector2.Lerp(fromPos, toPos, blend);

            yield return null;
        }

        target.localScale = toScale;
        target.anchoredPosition = toPos;
    }
}
