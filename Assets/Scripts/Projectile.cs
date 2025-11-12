using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 10f;
    public string enemyTag = "Enemy";
    public float destroyDelay = 0.05f;

    [Header("Movement")]
    public float speed = 10f;
    public Vector2 direction = Vector2.right;
    public float lifetime = 3f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.Translate(direction.normalized * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(enemyTag))
        {
            Debug.Log($"Bullet hit {other.name}");
            Destroy(gameObject, destroyDelay);
        }
    }
}
