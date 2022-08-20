using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour {
    public LayerMask GroundLayers;
    public Transform GroundBar;
    public UnityEvent OnLandEvent;
    public float speed = 40f;
    public float jump = 1100f;
    public float dash = 1200f;

    private Rigidbody2D rb;
    private Vector3 velocity = Vector3.zero;
    private bool facingRight = true, grounded = false, canDash = false;
    private bool jumping = false, dashing = false;
    private float x_input = 0f;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        x_input = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) {
            jumping = true;
        }
        if (Input.GetButtonDown("Dash") && canDash) {
            dashing = true;
        }
    }

    void FixedUpdate() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundBar.position, 0.15f, GroundLayers);
        for (int i = 0; i < colliders.Length; ++i) {
            if (colliders[i].gameObject != gameObject) {
                if (!grounded) {
                    grounded = true;
                    canDash = true;
                    OnLandEvent.Invoke();
                }
            }
        }

        Move(x_input * speed * Time.fixedDeltaTime);
        grounded = false;
        jumping = false;
        dashing = false;
    }

    private void Move(float m) {
        Vector3 targetVelocity = new Vector2(m * 10f, rb.velocity.y);
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, 0.1f);

        if (facingRight) {
            if (m < 0) {
                FlipDirection(false);
            }
        } else {
            if (m > 0) {
                FlipDirection(true);
            }
        }

        if (jumping && grounded) {
            grounded = false;
            rb.AddForce(new Vector2(0f, jump));
        }

        if (dashing) {
            rb.AddForce(new Vector2(dash * (facingRight ? 1 : -1), 0f));
            canDash = false;
        }
    }

    private void FlipDirection(bool toRight) {
        facingRight = toRight;
        Vector3 newDirection = transform.localScale;
        newDirection.x *= -1;
        transform.localScale = newDirection;
    }
}
