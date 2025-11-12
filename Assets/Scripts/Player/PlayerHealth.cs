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
    public float knockbackForce = 8f;     
    public float invincibleDuration = 2f;  
    public float flashInterval = 0.1f;  

    [Header("Enemy/Boss Tags")]
    public string[] damageTags = { "Enemy", "Boss" };

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInvincible) return;

        foreach (string tag in damageTags)
        {
            if (collision.collider.CompareTag(tag))
            {
                // calculate knockback
                Vector2 hitDir = (transform.position - collision.transform.position).normalized;
                TakeDamage(1, hitDir);
                break;
            }
        }
    }

    public void TakeDamage(int amount, Vector2 hitDir)
    {
        if (isInvincible) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"Player hit! HP: {currentHealth}/{maxHealth}");

        if (rb != null)
        {
            rb.velocity = Vector2.zero;

            Vector2 knockDir = -hitDir.normalized;
            if (Mathf.Abs(knockDir.x) < 0.1f)
                knockDir.x = Mathf.Sign(transform.localScale.x) * -1f; 

            knockDir = Quaternion.Euler(0, 0, 30f) * knockDir;

            float force = knockbackForce * 1.2f;
            rb.AddForce(knockDir * force, ForceMode2D.Impulse);

            Debug.Log($"Knockback: dir={knockDir}, force={force}");
        }

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
        Debug.Log("Player has died! Restarting level...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
