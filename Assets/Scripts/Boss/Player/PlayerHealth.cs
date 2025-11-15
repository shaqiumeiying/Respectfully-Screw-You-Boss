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

    public UIHeartsController heartsUI;

    private Rigidbody2D rb;
    private Collider2D col;
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

        //todo add player feedback

        // Find fade panel in scene
        FadeController fade = FindObjectOfType<FadeController>();

        if (fade != null)
            StartCoroutine(fade.FadeToWhiteAndLoad("Lose", 1.5f));
        else
            SceneManager.LoadScene("Lose");
    }

}
