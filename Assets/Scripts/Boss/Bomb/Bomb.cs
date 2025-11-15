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
        }

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}