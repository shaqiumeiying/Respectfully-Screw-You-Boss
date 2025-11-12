using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("General References")]
    public Transform firePoint;           // where attacks come from
    public GameObject projectilePrefab;   // projectile prefab
    public LayerMask enemyLayer;          // enemy layer in inspector

    [Header("Projectile Settings")]
    public float projectileSpeed = 15f;
    public float projectileCooldown = 0.3f;
    public float projectileDamage = 5f;   // projectile damage

    [Header("Melee Settings")]
    public float meleeRange = 1.5f;
    public float meleeCooldown = 0.4f;
    public float meleeRadius = 0.5f;
    public float meleeDamage = 10f;       // melee damage

    [Header("Audio")]
    public AudioClip shootSFX;
    public AudioClip meleeSFX;
    private AudioSource audioSource;

    private float nextProjectileTime;
    private float nextMeleeTime;
    private Camera mainCam;
    private Vector2 aimDirection;

    void Start()
    {
        mainCam = Camera.main;
        audioSource = GetComponent<AudioSource>();
        if (!audioSource)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void Update()
    {
        UpdateAimDirection();
        HandleAttackInput();
    }

    // ==================== INPUT ====================
    void HandleAttackInput()
    {
        float rt = Input.GetAxis("Fire1");
        float lt = Input.GetAxis("Fire2");

        bool shootPressed = rt > 0.5f || Input.GetButton("Fire1");
        bool meleePressed = lt > 0.5f || Input.GetButton("Fire2");

        if (shootPressed && Time.time >= nextProjectileTime)
        {
            nextProjectileTime = Time.time + projectileCooldown;
            ShootProjectile();
        }

        if (meleePressed && Time.time >= nextMeleeTime)
        {
            nextMeleeTime = Time.time + meleeCooldown;
            PerformMelee();
        }
    }

    // ==================== AIMING ====================
    void UpdateAimDirection()
    {
        if (!mainCam) return;

        Vector2 stick = new Vector2(Input.GetAxis("RightStickHorizontal"), Input.GetAxis("RightStickVertical"));
        if (stick.magnitude > 0.3f)
        {
            aimDirection = stick.normalized;
        }
        else
        {
            Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            aimDirection = (mousePos - transform.position);
            aimDirection.Normalize();
        }

        if (firePoint)
        {
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // ==================== PROJECTILE ====================
    void ShootProjectile()
    {
        if (!projectilePrefab || !firePoint) return;

        if (shootSFX) audioSource.PlayOneShot(shootSFX);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.velocity = aimDirection * projectileSpeed;
        }

        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
            p.damage = projectileDamage;

        Destroy(proj, 2f);
    }

    // ==================== MELEE ====================
    void PerformMelee()
    {
        if (meleeSFX) audioSource.PlayOneShot(meleeSFX);

        Vector2 origin = (Vector2)firePoint.position + aimDirection * meleeRange * 0.5f;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, meleeRadius, enemyLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Melee hit: " + hit.name);

            MinionBase minion = hit.GetComponent<MinionBase>();
            if (minion != null)
            {
                Vector2 hitDir = (minion.transform.position - transform.position).normalized;
                minion.TakeDamage(meleeDamage, hitDir);
                continue;
            }

            BossBase boss = hit.GetComponent<BossBase>();
            if (boss != null)
            {
                Vector2 hitDir = (boss.transform.position - transform.position).normalized;
                boss.TakeDamage(meleeDamage, hitDir);
                continue;
            }
        }
    }
}
