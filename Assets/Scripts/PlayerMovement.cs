using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;
    public float rollSpeed = 20f;
    public float rollDuration = 0.5f;
    public float doubleJumpVelocity = 10f;

    float horizontalMove = 0f;
    bool jump = false;
    bool crouch = false;

    private bool isRolling = false;
    private Rigidbody2D rb;
    private Collider2D playerCollider;
    private bool canDoubleJump = false;
    private bool hasDoubleJumped = false;

    private PlayerAudioAdvanced playerAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
        playerAudio = GetComponent<PlayerAudioAdvanced>();
    }

    void Update()
    {
        if (!isRolling)
        {
            horizontalMove = 0f;
            if (KeybindSettings.Instance.GetKey("WalkingLeft"))
            {
                horizontalMove = -runSpeed;
            }
            else if (KeybindSettings.Instance.GetKey("WalkingRight"))
            {
                horizontalMove = runSpeed;
            }

            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            if (KeybindSettings.Instance.GetKeyDown("Jump"))
            {
                if (controller.IsGrounded())
                {
                    jump = true;
                    hasDoubleJumped = false;
                    animator.SetTrigger("JumpTrigger");
                    playerAudio?.PlayJumpSound();
                }
                else if (canDoubleJump && !hasDoubleJumped)
                {
                    jump = true;
                    hasDoubleJumped = true;
                    animator.SetTrigger("JumpTrigger");
                    rb.velocity = new Vector2(rb.velocity.x, doubleJumpVelocity);
                    playerAudio?.PlayJumpSound();
                }
            }

            if (KeybindSettings.Instance.GetKeyDown("Crouch"))
            {
                crouch = true;
            }
            else if (KeybindSettings.Instance.GetKeyUp("Crouch"))
            {
                crouch = false;
            }

            if (KeybindSettings.Instance.GetKeyDown("Roll") && controller.IsGrounded())
            {
                StartRoll();
            }
        }

        if (playerAudio != null)
        {
            playerAudio.SetGrounded(controller.IsGrounded());
        }
    }

    void StartRoll()
    {
        if (!isRolling)
        {
            playerAudio?.SetRolling(true); // începe roll
            StartCoroutine(PerformRoll());
        }
    }

    private IEnumerator PerformRoll()
    {
        isRolling = true;
        float direction = controller.GetFacingDirection();
        animator.SetTrigger("Roll");

        int defaultLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("RollingPlayer");

        float timer = 0f;
        while (timer < rollDuration)
        {
            rb.velocity = new Vector2(direction * rollSpeed, rb.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        gameObject.layer = defaultLayer;
        isRolling = false;
        playerAudio?.SetRolling(false); // finalizează roll
    }

    public void OnLanding()
    {
        animator.ResetTrigger("JumpTrigger");

        if (Mathf.Abs(horizontalMove) > 0.01f)
            animator.Play("Player_run");
        else
            animator.Play("Player_idle");

        hasDoubleJumped = false;
        playerAudio?.SetGrounded(true);
    }

    void FixedUpdate()
    {
        if (!isRolling)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;

            if (controller.IsGrounded())
            {
                hasDoubleJumped = false;
            }
        }
    }

    public IEnumerator SpeedBoost(float multiplier, float duration)
    {
        float originalSpeed = runSpeed;
        runSpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        runSpeed = originalSpeed;
    }

    public void EnableDoubleJump()
    {
        canDoubleJump = true;
        Debug.Log("Double jump activated!");
    }

    public bool HasDoubleJumpAbility()
    {
        return canDoubleJump;
    }

    public void SetDoubleJumpAbility(bool value)
    {
        canDoubleJump = value;
        if (value)
        {
            Debug.Log("Double jump ability loaded!");
        }
    }
}
