using UnityEngine;
using UnityEngine.EventSystems;

public class HoverLines : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject lineTop;
    public GameObject lineBottom;

    public void OnPointerEnter(PointerEventData eventData)
    {
        lineTop.SetActive(true);
        lineBottom.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        lineTop.SetActive(false);
        lineBottom.SetActive(false);
    }
}
