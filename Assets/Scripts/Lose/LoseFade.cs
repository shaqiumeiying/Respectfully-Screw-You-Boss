using System.Collections;
using UnityEngine;

public class LoseFade : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip fail;

    public GameObject fadePanel;   // assign in Inspector

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        audioSource.PlayOneShot(fail);

        FadeController fade = FindObjectOfType<FadeController>();

        if (fade != null)
            StartCoroutine(FadeAndDisable(fade));
    }

    private IEnumerator FadeAndDisable(FadeController fade)
    {
        // Wait for the fade-out to finish
        yield return StartCoroutine(
            fade.Fade(
                new Color(1, 0, 0, 1),   // start: red opaque
                new Color(1, 0, 0, 0),   // end: red transparent
                0.6f                     // duration
            )
        );

        // Disable fade panel after animation
        fadePanel.SetActive(false);
    }
}
