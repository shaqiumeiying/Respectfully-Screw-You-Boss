using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 2f;
    public int explosionDamage = 2;      
    public LayerMask playerLayer;

    [Header("Audio")]
    public AudioClip explosionSFX;
    private AudioSource audioSource;

    [Header("References")]
    [HideInInspector] public GameObject shadowInstance;
    private SpriteRenderer shadowSR;

    private bool hasExploded = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        if (shadowInstance != null)
        {
            float height = transform.position.y;

            float scale = Mathf.Lerp(0.5f, 1.5f, Mathf.Clamp01(1 - (height / 8f)));
            shadowInstance.transform.localScale = new Vector3(scale, scale * 0.6f, 1f);

            shadowInstance.transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

            if (shadowSR != null)
            {
                Color c = shadowSR.color;
                c.a = Mathf.Lerp(0.3f, 0.9f, Mathf.Clamp01(1 - (height / 8f)));
                shadowSR.color = c;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasExploded) return;

        if (collision.collider.CompareTag("Ground") || collision.collider.CompareTag("Player"))
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
                Vector2 hitDir = (player.transform.position - transform.position).normalized;
                player.TakeDamage(explosionDamage, hitDir);
                Debug.Log("Player hit by bomb");
            }
        }


        //Animator anim = GetComponent<Animator>();
        //if (anim != null)
        //    anim.SetTrigger("Explode");

        Destroy(gameObject, 1f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}