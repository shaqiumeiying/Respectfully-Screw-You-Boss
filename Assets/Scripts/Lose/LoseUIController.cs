using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoseUIController : MonoBehaviour
{
    public Image fillBar;
    public TMP_Text percentTextbase;

    void Start()
    {
        float percent = BossBase.bossLastHealthPercent;
        SetBossProgress(percent);
    }

    public void SetBossProgress(float percent)
    {
        percent = Mathf.Clamp01(percent);
        fillBar.fillAmount = percent;
        percentTextbase.text = Mathf.RoundToInt(percent * 100f) + "%";
    }
}
