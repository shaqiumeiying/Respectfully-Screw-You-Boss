using System.Collections;
using UnityEngine;

public class WinFade : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip sucess;

    public GameObject fadePanel;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        audioSource.PlayOneShot(sucess);

        FadeController fade = FindObjectOfType<FadeController>();

        if (fade != null)
            StartCoroutine(FadeAndDisable(fade));
    }

    private IEnumerator FadeAndDisable(FadeController fade)
    {
        // Wait for the fade coroutine to complete
        yield return StartCoroutine(
            fade.Fade(
                new Color(1, 1, 1, 1),   // start white opaque
                new Color(1, 1, 1, 0),   // end transparent
                0.75f                     // duration
            )
        );

        // Disable fade panel after fade-out is finished
        fadePanel.SetActive(false);
    }
}
