using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Teleport Settings")]
    public Transform leftPoint;
    public Transform rightPoint;

    public float minTeleportTime = 6f;
    public float maxTeleportTime = 10f;

    public float shiftDistance = 0.5f; 
    public float shiftSpeed = 4f;     

    private bool onLeftSide = false;

    [Header("Attack Effect")]
    public GameObject attackEffectPrefab;
    public Transform attackPoint;


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

    public static float bossLastHealthPercent = 0f;

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
        StartCoroutine(TeleportLoop());
    }

    void FlipAttackPoint()
    {
        if (attackPoint == null) return;

        Vector3 p = attackPoint.localPosition;
        p.x *= -1;
        attackPoint.localPosition = p;
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
    IEnumerator TeleportLoop()
    {
        while (!isDead)
        {
            // Wait 6¨C10 seconds randomly
            float waitTime = Random.Range(minTeleportTime, maxTeleportTime);
            yield return new WaitForSeconds(waitTime);

            TeleportBoss();
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

        if (attackEffectPrefab && attackPoint)
        {
            GameObject effect = Instantiate(attackEffectPrefab, attackPoint.position, attackPoint.rotation);

            // Try SpriteRenderer flip
            SpriteRenderer fxSR = effect.GetComponent<SpriteRenderer>();
            if (fxSR != null)
            {
                fxSR.flipX = sr.flipX;
            }
            else
            {
                // fallback: flip by scale
                Vector3 scale = effect.transform.localScale;
                scale.x = Mathf.Abs(scale.x) * (sr.flipX ? -1f : 1f);
                effect.transform.localScale = scale;
            }
        }

        Vector2 playerPos = playerTarget.position;
        float distance = Vector2.Distance(playerPos, transform.position);
        if (distance > attackRadius)
        {
            Debug.Log("Player out of range, attack missed.");
        }
        else
        {
            float dir = playerPos.x >= transform.position.x ? 1f : -1f;

            PlayerHealth player = playerTarget.GetComponent<PlayerHealth>();
            if (player != null)
                player.TakeDamage((int)attackDamage);
        }

        if (attackSprite != null)
            sr.sprite = attackSprite;

        

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
        Vector3 basePos = transform.position;
        float elapsed = 0f;

        while (elapsed < jitterDuration)
        {
            Vector3 randomOffset = new Vector3(
                Random.Range(-jitterStrength, jitterStrength),
                Random.Range(-jitterStrength, jitterStrength),
                0);

            transform.position = basePos + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = basePos;
    }

    void TeleportBoss()
    {
        if (onLeftSide)
        {
            StartCoroutine(ShiftAndTeleport(rightPoint.position, shiftDirection: Vector3.left, flipAfter: false));
        }
        else
        {
            StartCoroutine(ShiftAndTeleport(leftPoint.position, shiftDirection: Vector3.right, flipAfter: true));
        }

        Debug.Log("Boss teleported to: " + (onLeftSide ? "LEFT" : "RIGHT"));
    }

    IEnumerator ShiftAndTeleport(Vector3 targetPosition, Vector3 shiftDirection, bool flipAfter)
    {
        Vector3 shiftTarget = transform.position + shiftDirection * shiftDistance;

        while (Vector3.Distance(transform.position, shiftTarget) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, shiftTarget, shiftSpeed * Time.deltaTime);
            yield return null;
        }

        yield return new WaitForSeconds(0.15f);

        transform.position = targetPosition;
        sr.flipX = flipAfter;
        onLeftSide = flipAfter;

        FlipAttackPoint();

        Color c = sr.color;
        c.a = 0f;
        sr.color = c;

        float t = 0f;
        float fadeDuration = 0.15f;  // very quick fade-in

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / fadeDuration);
            sr.color = c;
            yield return null;
        }

        // ensure fully visible
        c.a = 1f;
        sr.color = c;
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

        PlayerHealth ph = FindObjectOfType<PlayerHealth>();
        if (ph != null)
        {
            PlayerHealth.lastPlayerHealth = ph.CurrentHealth;
            Debug.Log("Saved player HP for Win Scene: " + PlayerHealth.lastPlayerHealth);
        }

        // --- SLOW MOTION ---
        Time.timeScale = 0.3f;

        // Find fade panel in scene
        FadeController fade = FindObjectOfType<FadeController>();

        if (fade != null)
            StartCoroutine(fade.Fade(
            new Color(1, 1, 1, 0),   // transparent white
            new Color(1, 1, 1, 1),   // fully white
            1.5f,                    // duration
            "Win"                    // scene to load
        ));
        else
            SceneManager.LoadScene("Win");
    }

    public float GetHealthPercent()
    {
        return currentHealth / maxHealth;
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
