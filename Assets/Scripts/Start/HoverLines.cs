using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;

public class HoverLines : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Line Objects")]
    public RectTransform lineTop;
    public RectTransform lineBottom;

    [Header("Animation Settings")]
    public float hoverAnimTime = 0.2f;       

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip startSound;

    [Header("Scene")]
    public string nextScene = "Dialogue";

    private bool isHovered = false;
    private bool sceneTriggered = false;

    void Start()
    {
        SetLineScale(0f);

        if (!audioSource)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (sceneTriggered) return;

        if (Input.GetKeyDown(KeyCode.Return) ||
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetMouseButtonDown(0) ||
            Input.anyKeyDown ||
            Input.GetKeyDown(KeyCode.JoystickButton0))
        {
            TriggerPress();
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (Mathf.Abs(x) > 0.2f || Mathf.Abs(y) > 0.2f)
            TriggerHover();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!sceneTriggered)
            TriggerHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!sceneTriggered)
            TriggerExit();
    }

    // -----------------------------------------
    void TriggerPress()
    {
        if (sceneTriggered) return;

        sceneTriggered = true;

        StopAllCoroutines();

        SetLineScale(1f);

        if (startSound)
            audioSource.PlayOneShot(startSound);

        float delay = startSound ? startSound.length : 0.5f;
        Invoke(nameof(GoNextScene), delay);
    }

    void GoNextScene()
    {
        SceneManager.LoadScene(nextScene);
    }

    // -----------------------------------------
    void TriggerHover()
    {
        if (sceneTriggered || isHovered) return;
        isHovered = true;

        StopAllCoroutines();
        StartCoroutine(ScaleLines(0f, 1f));
    }

    void TriggerExit()
    {
        if (sceneTriggered || !isHovered) return;
        isHovered = false;

        StopAllCoroutines();
        StartCoroutine(ScaleLines(1f, 0f));
    }

    // -----------------------------------------
    IEnumerator ScaleLines(float from, float to)
    {
        float t = 0f;
        while (t < hoverAnimTime)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(from, to, t / hoverAnimTime);
            SetLineScale(v);
            yield return null;
        }
        SetLineScale(to);
    }

    private void SetLineScale(float x)
    {
        if (lineTop)
            lineTop.localScale = new Vector3(x, 1, 1);
        if (lineBottom)
            lineBottom.localScale = new Vector3(x, 1, 1);
    }
}
