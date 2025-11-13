using UnityEngine;

public class MinionHealthBar : MonoBehaviour
{
    [Header("References")]
    public Transform fillBar; // The green bar
    public MinionBase minion; // The minion this bar follows

    private Vector3 originalScale;
    private Vector3 originalPos;

    void Start()
    {
        if (fillBar != null)
        {
            originalScale = fillBar.localScale;
            originalPos = fillBar.localPosition;
        }
    }

    void Update()
    {
        if (minion == null || fillBar == null) return;

        float ratio = Mathf.Clamp01(minion.GetHealthRatio());

        fillBar.localScale = new Vector3(originalScale.x * ratio, originalScale.y, originalScale.z);

        float offset = (1 - ratio) * -0.5f * originalScale.x;
        fillBar.localPosition = originalPos + new Vector3(offset, 0, 0);
    }
}
