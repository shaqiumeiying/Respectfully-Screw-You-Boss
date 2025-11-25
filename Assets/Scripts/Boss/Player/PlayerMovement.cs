using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpForce = 7f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Knockback")]
    public bool isKnockedback = false;
    public float knockbackDuration = 0.2f;

    private Animator anim;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private bool isGrounded;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // ----- STOP movement when knocked -----
        if (isKnockedback)
            return;

        float move = Input.GetAxis("Horizontal");

        // Movement
        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }

        // Flip + Animations
        if (move > 0.1f)
            sr.flipX = false;
        else if (move < -0.1f)
            sr.flipX = true;

        anim.SetFloat("Speed", Mathf.Abs(move));

        // ---- Better Jump ----
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        StartCoroutine(KnockbackRoutine(force));
    }

    IEnumerator KnockbackRoutine(Vector2 force)
    {
        isKnockedback = true;
        rb.velocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedback = false;
    }

    // Ground checking
    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
            isGrounded = false;
    }
}
