using System.Collections;
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
    private Rigidbody2D rb;
    private Collider2D playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (!isRolling)
        {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

            if (Input.GetButtonDown("Jump"))
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }

            if (Input.GetButtonDown("Crouch"))
            {
                crouch = true;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                crouch = false;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && controller.IsGrounded())
            {
                StartRoll();
            }
        }
    }

    void StartRoll()
    {
        if (!isRolling)
        {
            StartCoroutine(PerformRoll());
        }
    }

    private IEnumerator PerformRoll()
    {
        isRolling = true;
        float direction = controller.GetFacingDirection();
        animator.SetTrigger("Roll");

        // 🔄 Schimbă layer-ul temporar
        int defaultLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("RollingPlayer");

        float timer = 0f;
        while (timer < rollDuration)
        {
            rb.velocity = new Vector2(direction * rollSpeed, rb.velocity.y);
            timer += Time.deltaTime;
            yield return null;
        }

        // 🔄 Revine la layer-ul original
        gameObject.layer = defaultLayer;

        isRolling = false;
    }


    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    void FixedUpdate()
    {
        if (!isRolling)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
            jump = false;
        }
    }

    public IEnumerator SpeedBoost(float multiplier, float duration)
    {
        float originalSpeed = runSpeed;
        runSpeed *= multiplier;

        yield return new WaitForSeconds(duration);

        runSpeed = originalSpeed;
    }
}
