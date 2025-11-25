using System.Collections;
using UnityEngine;

public class MinionPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float leftBoundary = -8f;
    public float rightBoundary = 8f;

    [Header("Swarm Settings")]
    public float swarmSpeed = 3f;
    public float playerDetectRange = 6f;
    public float minDistanceToPlayer = 1.2f;
    public float randomOffsetRange = 1.0f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool movingLeft = true;
    private bool isDead = false;

    private Transform player;
    private float randomOffset;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        randomOffset = Random.Range(-randomOffsetRange, randomOffsetRange);
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        //if (distanceToPlayer < playerDetectRange)
        //{
        //    SwarmToPlayer();
        //}
        //else
        //{
        //    Patrol();
        //}
        Patrol();
    }

    void Patrol()
    {
        float moveDir = movingLeft ? -1f : 1f;

        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        if (sr)
            sr.flipX = moveDir > 0;

        if (transform.position.x < leftBoundary) movingLeft = false;
        if (transform.position.x > rightBoundary) movingLeft = true;
    }

    void SwarmToPlayer()
    {
        float targetX = player.position.x + randomOffset;
        float diff = targetX - transform.position.x;

        if (Mathf.Abs(diff) < minDistanceToPlayer)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        float direction = Mathf.Sign(diff);
        rb.velocity = new Vector2(direction * swarmSpeed, rb.velocity.y);

        if (sr)
            sr.flipX = direction > 0;
    }

    public void OnDeath()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
    }
}
