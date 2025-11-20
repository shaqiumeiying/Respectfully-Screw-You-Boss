using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WinUIController : MonoBehaviour
{
    public TMP_Text playerHpText;

    void Start()
    {
        playerHpText.text = PlayerHealth.lastPlayerHealth.ToString();
    }
}
