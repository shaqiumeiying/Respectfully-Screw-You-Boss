//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerMovement : MonoBehaviour
//{
//    public float moveSpeed = 5f;
//    public float jumpForce = 7f;

//    private Animator anim;
//    private Rigidbody2D rb;
//    private SpriteRenderer sr;
//    private bool isGrounded;

//    void Start()
//    {
//        sr = GetComponent<SpriteRenderer>();
//        anim = GetComponent<Animator>();
//        rb = GetComponent<Rigidbody2D>();
//    }

//    void Update()
//    {
//        float move = Input.GetAxis("Horizontal");

//        // movement
//        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

//        // jump
//        if (Input.GetButtonDown("Jump") && isGrounded)
//        {
//            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
//            anim.SetTrigger("Jump");
//        }

//        // flip sprite
//        if (move > 0.1f)
//            sr.flipX = false;
//        else if (move < -0.1f)
//            sr.flipX = true;

//        // running / idle animation
//        anim.SetFloat("Speed", Mathf.Abs(move));
//    }

//    void OnCollisionEnter2D(Collision2D col)
//    {
//        if (col.gameObject.CompareTag("Ground"))
//            isGrounded = true;

//    }
//    void OnCollisionExit2D(Collision2D col)
//    {
//        if (col.gameObject.CompareTag("Ground"))
//            isGrounded = false;
//    }
//}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("Better Jump Settings")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

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
        float move = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(move * moveSpeed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }

        if (move > 0.1f)
            sr.flipX = false;
        else if (move < -0.1f)
            sr.flipX = true;

        anim.SetFloat("Speed", Mathf.Abs(move));

        if (rb.velocity.y < 0) // Falling
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump")) // Short hop
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

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
