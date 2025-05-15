using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;

    public float rollSpeed = 20f;
    public float rollDuration = 0.5f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    private bool isRolling = false;
    private float rollTimer = 0f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Disable movement input while rolling
        if (!isRolling)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        }

        // Jump
        if (Input.GetButtonDown("Jump") && !isRolling)
        {
            jump = true;
            animator.SetBool("IsJumping", true);
        }

        // Crouch
        if (Input.GetButtonDown("Crouch") && !isRolling)
        {
            crouch = true;
        }
        else if (Input.GetButtonUp("Crouch"))
        {
            crouch = false;
        }

        // Roll (Left Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isRolling && controller.IsGrounded())
        {
            StartRoll();
        }
    }

    void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;
        animator.SetTrigger("Roll"); // folosim Trigger în loc de SetBool
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        if (isRolling)
        {
            float direction = controller.GetFacingDirection(); // +1 sau -1
            rb.velocity = new Vector2(direction * rollSpeed, rb.velocity.y);

            rollTimer -= Time.fixedDeltaTime;
            if (rollTimer <= 0f)
            {
                isRolling = false;
            }

            return; // skip normal movement while rolling
        }

        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}