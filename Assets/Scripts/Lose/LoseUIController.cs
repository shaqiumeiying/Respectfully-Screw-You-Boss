using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoseUIController : MonoBehaviour
{
    public Image fillBar;
    public TMP_Text percentText;

    void Start()
    {
        float percent = BossBase.bossLastHealthPercent;
        SetBossProgress(percent);
    }

    public void SetBossProgress(float percent)
    {
        percent = Mathf.Clamp01(percent);
        fillBar.fillAmount = percent;
        percentText.text = Mathf.RoundToInt(percent * 100f) + "%";
    }
}
