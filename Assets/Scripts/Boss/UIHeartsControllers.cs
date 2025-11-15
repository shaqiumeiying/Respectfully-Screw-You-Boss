using UnityEngine;
using UnityEngine.UI;

public class UIHeartsController : MonoBehaviour
{
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Image[] hearts;   // assign in inspector

    public void UpdateHearts(int currentHP, int maxHP)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHP)
                hearts[i].sprite = fullHeart;
            else
                hearts[i].sprite = emptyHeart;
        }
    }
}
