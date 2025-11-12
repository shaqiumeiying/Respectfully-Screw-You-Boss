using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float leftBoundary = -8f;
    public float rightBoundary = 8f;

    private Rigidbody2D rb;
    private bool movingLeft = true;
    private SpriteRenderer sr;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isDead) return;

        float moveDir = movingLeft ? -1f : 1f;
        rb.velocity = new Vector2(moveDir * moveSpeed, rb.velocity.y);

        //Flip
        if (sr)
            sr.flipX = movingLeft;
        if (transform.position.x < leftBoundary) movingLeft = false;
        if (transform.position.x > rightBoundary) movingLeft = true;
    }

    public void OnDeath()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
    }
}
