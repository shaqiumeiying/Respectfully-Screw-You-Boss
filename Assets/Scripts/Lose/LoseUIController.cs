using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoseUIController : MonoBehaviour
{
    public Image fillBar;
    public TMP_Text percentTextbase;
    public TMP_Text percentTextbase1;
    public TMP_Text percentTextbase2;

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
        percentTextbase1.text = Mathf.RoundToInt(percent * 100f) + "%";
        percentTextbase2.text = Mathf.RoundToInt(percent * 100f) + "%";
    }
}
