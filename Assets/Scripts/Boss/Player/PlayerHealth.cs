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


            if (collision.collider.CompareTag("Enemy"))
            {
                // calculate knockback
                Vector2 hitDir = (transform.position - collision.transform.position).normalized;
                TakeDamage(1, hitDir);
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
            Vector2 knockDir = hitDir == default ? Vector2.up : hitDir.normalized;

            Vector2 knockback = new Vector2(
                knockDir.x + knockbackForce,
                Mathf.Abs(knockDir.y) + 5f
            );

            rb.AddForce(knockback, ForceMode2D.Impulse);
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
