using System.Collections;
using UnityEngine;

public class BlinkingPlatform : MonoBehaviour
{
    public SpriteRenderer sr;
    public Collider2D col;

    public int blinkCount = 3;
    public float blinkSpeed = 0.12f;

    public float dropDistance = 5f;      // how far it falls
    public float dropSpeed = 6f;         // how fast it falls
    public float respawnDelay = 2f;

    private bool isTriggered = false;
    private Vector3 originalPos;

    void Start()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        if (!col) col = GetComponent<Collider2D>();

        originalPos = transform.position; // save start position
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (isTriggered) return;
        if (!collision.collider.CompareTag("Player")) return;

        Transform player = collision.collider.transform;

        // Player must be ABOVE platform
        if (player.position.y > transform.position.y + 0.2f)
        {
            StartCoroutine(BlinkDropRespawn());
        }
    }

    IEnumerator BlinkDropRespawn()
    {
        isTriggered = true;
        
        //wait time
        yield return new WaitForSeconds(2f);

        // 1. BLINK
        for (int i = 0; i < blinkCount; i++)
        {
            SetAlpha(0f);
            yield return new WaitForSeconds(blinkSpeed);
            SetAlpha(1f);
            yield return new WaitForSeconds(blinkSpeed);
        }

        // 2. DROP (smooth downward)
        float dropped = 0f;
        Vector3 start = transform.position;
        Vector3 end = start + Vector3.down * dropDistance;

        while (dropped < dropDistance)
        {
            float step = dropSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, end, step);
            dropped += step;
            yield return null;
        }

        // 3. DISAPPEAR
        sr.enabled = false;
        col.enabled = false;

        // 4. WAIT
        yield return new WaitForSeconds(respawnDelay);

        // 5. RESET POSITION
        transform.position = originalPos;

        // 6. REAPPEAR
        sr.enabled = true;
        col.enabled = true;

        isTriggered = false;
    }

    void SetAlpha(float a)
    {
        Color c = sr.color;
        c.a = a;
        sr.color = c;
    }
}
