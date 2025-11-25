using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 2f;
    public int explosionDamage = 2;      
    public LayerMask playerLayer;

    [Header("Audio")]
    public AudioClip fusingSFX;
    public AudioClip explosionSFX;
    private AudioSource audioSource;

    [Header("References")]
    [HideInInspector] public GameObject shadowInstance;
    private SpriteRenderer shadowSR;
    private Vector3 baseScale;

    private bool hasExploded = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (shadowInstance != null)
            baseScale = shadowInstance.transform.localScale;   // store it once
            shadowSR = shadowInstance.GetComponent<SpriteRenderer>();   
        Physics2D.IgnoreCollision(
            GetComponent<Collider2D>(),
            GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>(),
            true);
    }

    void Update()
    {
        if (shadowInstance != null)
        {
            float height = transform.position.y;
            float t = Mathf.Clamp01(1 - (height / 8f)); 

            float factor = Mathf.Lerp(0.4f, 2f, t);   

            shadowInstance.transform.localScale = baseScale * factor;

        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;

        if (collision.collider.CompareTag("Ground"))
        {
            Debug.Log("Bomb collided with " + collision.collider.tag);
            Explode();
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasExploded) return;

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Bomb Trigger hit player");
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Debug.Log("Bomb exploded!");

        if (explosionSFX)
            audioSource.PlayOneShot(explosionSFX);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, playerLayer);
        foreach (Collider2D hit in hits)
        {
            PlayerHealth player = hit.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(explosionDamage);
                Debug.Log("Player hit by bomb");
            }

            PlayerMovement pm = hit.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;
                dir.y = 1.2f;
                dir.x = 1.2f;
                dir.Normalize();
                //Knockback force by bomb
                pm.ApplyKnockback(dir * 10f);
            }
        }

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}