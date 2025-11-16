using System.Collections;
using UnityEngine;

public class MinionBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 10f;
    private float currentHealth;

    [Header("Death Settings")]
    public float destroyDelay = 3f;
    public float deathGravity = 2f;

    [Header("Hit Reaction")]
    public float knockbackForce = 10f;
    public float knockupForce = 6f;
    public float flashTime = 0.1f;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void TakeDamage(float amount, Vector2 hitDirection = default)
    {

        StartCoroutine(FlashRed());

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            Vector2 knockDir = hitDirection == default ? Vector2.up : hitDirection.normalized;
            Vector2 knockback = new Vector2(knockDir.x * knockbackForce, Mathf.Abs(knockDir.y) + knockupForce);
            rb.AddForce(knockback, ForceMode2D.Impulse);
        }

        if (isDead) return;

        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            StartCoroutine(DieAfterDelay(0.05f));
        }
    }

    IEnumerator DieAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (col) col.enabled = false;

        if (rb)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = deathGravity;
            rb.constraints = RigidbodyConstraints2D.None;
        }

        Destroy(gameObject, destroyDelay);
    }

    IEnumerator FlashRed()
    {
        if (sr == null) yield break;
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        sr.color = original;
    }


    public float GetHealthRatio()
    {
        return currentHealth / maxHealth;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Projectile proj = other.GetComponent<Projectile>();
        if (proj)
        {
            Vector2 hitDir = (transform.position - proj.transform.position).normalized;
            TakeDamage(proj.damage, hitDir);
            Destroy(proj.gameObject);
        }
    }
}
