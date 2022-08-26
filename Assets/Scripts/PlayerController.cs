using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {
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

    private int coinsCollected = 0;

    private float swingTime, swingStartTime = 0.5f, swingRange = 0.8f;
    private bool swinging;
    public Transform SwingPos;
    public LayerMask DoorLayer;
    public float doorBoost;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        x_input = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) {
            jumping = true;
        }
        if (Input.GetButtonDown("Dash")) {
            dashing = true;
        }
        if (Input.GetKeyDown(KeyCode.Z)) {
            swinging = true;
        }
    }

    void FixedUpdate() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(GroundBar.position, 0.15f, GroundLayers);
        for (int i = 0; i < colliders.Length; ++i) {
            if (colliders[i].gameObject != gameObject) {
                grounded = true;
                canDash = true;
                OnLandEvent.Invoke();
                break;
            }
        }
        if (swingTime <= 0) {
            if (swinging) {
                Collider2D doorCollider = Physics2D.OverlapCircle(SwingPos.position, swingRange, DoorLayer);
                if (doorCollider != null) {
                    onDoorHit(doorCollider);
                }
                swingTime = swingStartTime;
            }
        } else {
            swingTime -= Time.deltaTime;
        }

        Move(x_input * speed * Time.fixedDeltaTime);
        grounded = false;
        jumping = false;
        dashing = false;
        swinging = false;
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
            rb.AddForce(new Vector2(0f, jump));
        }

        if (dashing && canDash) {
            rb.AddForce(new Vector2(dash * (facingRight ? 1 : -1), 0f));

            if (!grounded) {
                canDash = false;
            }
        }
    }

    private void FlipDirection(bool toRight) {
        facingRight = toRight;
        Vector3 newDirection = transform.localScale;
        newDirection.x *= -1;
        transform.localScale = newDirection;
    }

    private void onDoorHit(Collider2D door) {
        rb.AddForce(new Vector2(0f, doorBoost));
        GameObject.Destroy(door.gameObject);
        canDash = true;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(SwingPos.position, swingRange);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Spike") {
            gameObject.GetComponent<Collider2D>().enabled = false;
            this.enabled = false;
        }

        if (collision.gameObject.layer == 6) { // Layer 6 is "Coins"
            Destroy(collision.gameObject);
            ++coinsCollected;
        }
    }
}
