using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public float typeSpeed = 0.03f;

    public SlideUI portraitSlide;
    public SlideUI boxSlide;

    private int index = 0;
    private bool isTyping = false;

    [TextArea(3, 10)]
    public string[] lines;

    [Header("Typing Sound")]
    public AudioSource audioSource;
    public AudioClip typeSound;
    public float pitchVariance = 0.05f;
    public float soundCooldown = 0.02f;
    private float lastSoundTime = 0f;


    void Start()
    {
        // Slide UI into view
        portraitSlide.SlideIn();
        boxSlide.SlideIn();

        index = 0;
        StartCoroutine(TypeLine());
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.anyKeyDown)
        {
            if (isTyping)
            {
                StopAllCoroutines();
                dialogueText.text = lines[index];
                isTyping = false;
            }
            else
            {
                NextLine();
            }
        }
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in lines[index])
        {
            dialogueText.text += c;

            if (c != ' ' && c != '\n' && typeSound != null && audioSource != null)
            {
                if (Time.time - lastSoundTime > soundCooldown)
                {
                    audioSource.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
                    audioSource.PlayOneShot(typeSound);
                    lastSoundTime = Time.time;
                }
            }

            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }

    void NextLine()
    {
        index++;

        if (index < lines.Length)
        {
            StartCoroutine(TypeLine());
        }
        else
        {
            portraitSlide.SlideOut(() =>
            {
                boxSlide.SlideOut(() => SceneManager.LoadScene("Boss"));
            });
        }
    }
}
