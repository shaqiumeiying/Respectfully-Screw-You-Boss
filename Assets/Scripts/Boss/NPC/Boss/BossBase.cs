using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBase : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 200f;
    private float currentHealth;

    [Header("Attack Settings")]
    public float attackInterval = 2.5f;  
    public float attackDelay = 0.5f;     
    public float attackRadius = 3f;      
    public float attackDamage = 10f;    
    public float detectRange = 6f;       
    public LayerMask playerLayer;
    public AudioClip gruntSound;
    public AudioClip attackSFX;

    [Header("Hit Reaction")]
    public float flashTime = 0.1f;
    public float jitterDuration = 0.15f;
    public float jitterStrength = 0.1f;

    [Header("Death Settings")]
    public float deathDelay = 3f;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Collider2D col;
    private AudioSource audioSource;
    private bool isDead = false;
    private Vector3 originalPosition;
    private Color originalColor;
    private Transform playerTarget;
    public BombSpawner bombSpawner;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Sprite attackSprite;

    void Start()
    {
        currentHealth = maxHealth;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        originalPosition = transform.localPosition;

        if (sr != null)
            originalColor = sr.color;

        StartCoroutine(BossBehaviorLoop());
    }

    IEnumerator BossBehaviorLoop()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(attackInterval);


            Collider2D playerHit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
            if (playerHit != null)
            {
                playerTarget = playerHit.transform;
                Debug.Log("Player detected! Boss preparing melee attack...");
                if (gruntSound && audioSource)
                    audioSource.PlayOneShot(gruntSound);

                yield return StartCoroutine(PerformMeleeAttack());
            }
            else
            {

                if (bombSpawner != null)
                {
                    bombSpawner.SpawnBomb();
                    Debug.Log("Player far away ¡ª bomb attack...");
                }
            }
        }
    }

    IEnumerator PerformMeleeAttack()
    {
        if (isDead || playerTarget == null) yield break;

        sr.color = new Color(0.9725f, 0.7843f, 0.7843f);
        yield return new WaitForSeconds(attackDelay);
        sr.color = originalColor;

        if (attackSFX && audioSource)
            audioSource.PlayOneShot(attackSFX);

        Vector2 playerPos = playerTarget.position;
        float distance = Vector2.Distance(playerPos, transform.position);
        if (distance > attackRadius)
        {
            Debug.Log("Player out of range, attack missed.");
        }

        if (attackSprite != null)
            sr.sprite = attackSprite;

        float dir = playerPos.x >= transform.position.x ? 1f : -1f;
        Vector2 hitDir = new Vector2(dir, 1f).normalized;

        PlayerHealth player = playerTarget.GetComponent<PlayerHealth>();
        if (player != null)
            player.TakeDamage((int)attackDamage, hitDir);

        yield return new WaitForSeconds(0.15f);

        if (idleSprite != null)
            sr.sprite = idleSprite;
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        StartCoroutine(FlashRed());
        StartCoroutine(JitterEffect());

        if (currentHealth <= 0)
            Die();
    }

    IEnumerator FlashRed()
    {
        if (sr == null) yield break;
        Color original = sr.color;
        sr.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        sr.color = original;
    }

    IEnumerator JitterEffect()
    {
        float elapsed = 0f;
        while (elapsed < jitterDuration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-jitterStrength, jitterStrength),
                Random.Range(-jitterStrength, jitterStrength),
                0);
            transform.localPosition = originalPosition + randomOffset;
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Boss defeated!");
        StopAllCoroutines();

        if (col) col.enabled = false;
        if (rb)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.gravityScale = 2f;
        }

        Destroy(gameObject, deathDelay);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 hitDir = Vector2.zero;
        Projectile proj = other.GetComponent<Projectile>();
        if (proj)
        {
            TakeDamage(proj.damage);
            Destroy(proj.gameObject);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
