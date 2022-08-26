using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D rb;
    private Animator anim;


    private LayerMask groundLayer;
    private LayerMask doorLayer;
    private Transform groundedPos, swingPos;
    private float speed = 250, jumpPower = 450, dashPower = 20, doorBoost = 12;
    private float groundedRadius = 0.1f, swingRange = 1f, swingTime, swingStartTime = 0.5f;
    private bool jumping = false, facingRight = true, isGrounded = false, canSwing = true, canDash = true, dashing = false, hasLanded = false, hasPlayedLandSound;

    private AudioSource audioSource;
    [SerializeField] AudioClip swing, dash, landed;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        audioSource = GetComponentInChildren<AudioSource>();
        groundLayer = LayerMask.GetMask("Ground");
        doorLayer = LayerMask.GetMask("Door");
        groundedPos = transform.GetChild(1);
        swingPos = transform.GetChild(2);

    }

    private void Update()
    {
        anim.SetFloat("speed", Mathf.Abs(rb.velocity.x));
        anim.SetBool("jumping", jumping);
        
        if (swingTime < 0)
        {
            swingTime = 0;
            canSwing = true;
        }
        else if(swingTime > 0)
        {
            swingTime -= 1 * Time.deltaTime;
            canSwing = false;
        }

        if(isGrounded)
        {
            hasLanded = true;
        }
        if(isGrounded && hasLanded && !hasPlayedLandSound)
        {
            audioSource.clip = landed;
            audioSource.Play();
            hasPlayedLandSound = true;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            if (canSwing)
            {
                canSwing = false;
                anim.SetTrigger("swing");
                audioSource.clip = swing;
                audioSource.Play();
                Collider2D doorHit = Physics2D.OverlapCircle(swingPos.position, swingRange, doorLayer);
                if (doorHit != null)
                {
                    onDoorHit(doorHit);
                }
                swingTime = swingStartTime;
            }
        }else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (canDash)
            {
                StartCoroutine(Dash());
            }
        }
    }

    private void FixedUpdate()
    {
       
        isGrounded = Physics2D.OverlapCircle(groundedPos.position, groundedRadius, groundLayer);

        if (isGrounded)
        {
            canDash = true;
        }

        float x_input = Input.GetAxisRaw("Horizontal");
        float y_input = Input.GetAxisRaw("Vertical");
           
        if (x_input > 0 && !dashing)
        {
            rb.velocity = new Vector3(speed * Time.fixedDeltaTime, rb.velocity.y, 0f);
            if (!facingRight)
                FlipDirection(true);

        }
        else if(x_input < 0 && !dashing)
        {
            rb.velocity = new Vector3(-speed * Time.fixedDeltaTime, rb.velocity.y, 0f);
            if (facingRight)
                FlipDirection(false);
        }
        else if(!dashing)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

        if(y_input > 0 && isGrounded)
        {
            jumping = true;
            rb.velocity = new Vector3(rb.velocity.x, jumpPower * Time.deltaTime, 0f);
        }
        else if(isGrounded)
        {
            jumping = false;
        }
        else if (!isGrounded)
        {
            hasLanded = false;
            hasPlayedLandSound = false;
        }
    }

    private void FlipDirection(bool toRight)
    {
        facingRight = toRight;
        Vector3 newDirection = transform.localScale;
        newDirection.x *= -1;
        transform.localScale = newDirection;
    }

    private void onDoorHit(Collider2D door)
    {
        rb.velocity = new Vector3(rb.velocity.x, doorBoost, 0f);
        door.gameObject.SetActive(false);
        canDash = true;
    }

    IEnumerator Dash()
    {
        dashing = true;
        rb.velocity = new Vector3(dashPower * (facingRight ? 1 : -1), 0f, 0f);
        rb.gravityScale = 0;
        anim.SetTrigger("dash");
        audioSource.clip = dash;
        audioSource.Play();
        canDash = false;
        yield return new WaitForSeconds(0.2f);
        rb.gravityScale = 2.5f;
        dashing = false;
    }

}
