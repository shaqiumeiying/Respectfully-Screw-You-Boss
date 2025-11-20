using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isInvincible = false;

    [Header("Hit Reaction")]
    public float knockbackForce = 200f;     
    public float invincibleDuration = 2f;  
    public float flashInterval = 0.1f;

    [Header("Ouch Sounds")]
    public AudioClip[] ouchSounds;
    private AudioSource audioSource;


    public UIHeartsController heartsUI;

    public int CurrentHealth => currentHealth;
    public static float lastPlayerHealth;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible) return;


            if (collision.collider.CompareTag("Enemy"))
            {
            
                Vector2 knockDir = new Vector2(-1f, 1f).normalized;
                ApplyKnockback(knockDir);
                TakeDamage(1);
            }

    }

    void ApplyKnockback(Vector2 dir)
    {
        rb.velocity = Vector2.zero;
        rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        Debug.Log($"Player knocked back! {dir}" );
    }


    public void TakeDamage(int amount)
    {
        if (isInvincible) return;

        PlayRandomOuch();

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"Player hit! HP: {currentHealth}/{maxHealth}");

        heartsUI.UpdateHearts(currentHealth, maxHealth);

        if (currentHealth <= 0)
            Die();
        else
            StartCoroutine(InvincibilityFlash());
    }


    System.Collections.IEnumerator InvincibilityFlash()
    {
        isInvincible = true;
        float timer = 0f;

        while (timer < invincibleDuration)
        {
            if (sr)
                sr.enabled = !sr.enabled; // flashing

            yield return new WaitForSeconds(flashInterval);
            timer += flashInterval;
        }

        if (sr)
            sr.enabled = true;

        isInvincible = false;
    }

    void Die()
    {
        
        Debug.Log("Player has died!");
        BossBase boss = FindObjectOfType<BossBase>();

        if (boss != null)
        {
            float percent = boss.GetHealthPercent();
            BossBase.bossLastHealthPercent = percent;
        }

        // --- SLOW MOTION ---
        Time.timeScale = 0.6f;

        // Find fade panel in scene
        FadeController fade = FindObjectOfType<FadeController>();

        if (fade != null)
            StartCoroutine(fade.Fade(
        new Color(1, 0, 0, 0),
        new Color(1, 0, 0, 1),
        0.5f,
        "Lose"
    ));
        else
            SceneManager.LoadScene("Lose");
    }

    private void PlayRandomOuch()
    {
        if (ouchSounds != null && ouchSounds.Length > 0)
        {
            int index = Random.Range(0, ouchSounds.Length);
            audioSource.PlayOneShot(ouchSounds[index]);
        }
    }


}
