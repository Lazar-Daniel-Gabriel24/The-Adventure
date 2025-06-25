using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;
    // [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f; // REMOVE OR COMMENT OUT THIS LINE
    [SerializeField] private bool m_AirControl = false;
    [SerializeField] private LayerMask m_WhatIsGround;
    [SerializeField] private Transform m_GroundCheckLeft;
    [SerializeField] private Transform m_GroundCheckRight;
    [SerializeField] private Transform m_CeilingCheck;
    [SerializeField] private Collider2D m_CrouchDisableCollider;

    // --- NEW: Add a variable for acceleration force ---
    [SerializeField] private float m_MoveAccelerationForce = 1500f; // Tune this value! Higher = faster acceleration

    const float k_GroundedRadius = .2f;
    const float k_CeilingRadius = .2f;
    private bool m_Grounded;
    private bool m_WasInAir = false;
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;
    // private Vector3 m_Velocity = Vector3.zero; // REMOVE OR COMMENT OUT IF NOT USED ELSEWHERE

    [Header("Events")]
    [Space]
    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }

    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        bool leftGrounded = CheckGround(m_GroundCheckLeft);
        bool rightGrounded = CheckGround(m_GroundCheckRight);

        m_Grounded = leftGrounded || rightGrounded;

        if (m_Grounded && m_WasInAir && m_Rigidbody2D.velocity.y <= 0f)
        {
            OnLandEvent.Invoke();
            m_WasInAir = false;
        }

        if (!m_Grounded)
        {
            m_WasInAir = true;
        }
    }

    private bool CheckGround(Transform groundCheck)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // If player is not grounded, and air control is off, exit early.
        // This prevents applying horizontal force when airControl is false and player is in air.
        if (!m_Grounded && !m_AirControl)
        {
            // Handle jump if it's not a double jump scenario already handled
            if (m_Grounded && jump) // Redundant check, but good for clarity if logic shifts
            {
                m_Grounded = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
            return; // Exit if no ground and no air control
        }


        if (!crouch)
        {
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        // Crouch logic remains the same
        if (crouch)
        {
            if (!m_wasCrouching)
            {
                m_wasCrouching = true;
                OnCrouchEvent.Invoke(true);
            }

            move *= m_CrouchSpeed;

            if (m_CrouchDisableCollider != null)
                m_CrouchDisableCollider.enabled = false;
        }
        else
        {
            if (m_CrouchDisableCollider != null)
                m_CrouchDisableCollider.enabled = true;

            if (m_wasCrouching)
            {
                m_wasCrouching = false;
                OnCrouchEvent.Invoke(false);
            }
        }

        // --- THE MAIN CHANGE IS HERE ---
        // Instead of SmoothDamp, apply force based on input
        // You multiply 'move' by 10f in the original targetVelocity calculation,
        // so let's try to maintain that scale with accelerationForce.
        float currentHorizontalSpeed = m_Rigidbody2D.velocity.x;
        float desiredHorizontalSpeed = move * 10f; // Assuming `move` is already normalized by runSpeed in PlayerMovement.cs

        // Calculate the difference between desired speed and current speed
        float speedDifference = desiredHorizontalSpeed - currentHorizontalSpeed;

        // Apply force to reach the desired speed
        // Use ForceMode2D.Force for continuous acceleration
        m_Rigidbody2D.AddForce(new Vector2(speedDifference * m_MoveAccelerationForce * Time.fixedDeltaTime, 0));
        // You might need to clamp the velocity to prevent it from going too fast, if `runSpeed` is the cap
        // if (Mathf.Abs(m_Rigidbody2D.velocity.x) > Mathf.Abs(desiredHorizontalSpeed) && Mathf.Sign(m_Rigidbody2D.velocity.x) == Mathf.Sign(desiredHorizontalSpeed))
        // {
        //     m_Rigidbody2D.velocity = new Vector2(desiredHorizontalSpeed, m_Rigidbody2D.velocity.y);
        // }


        if (move > 0 && !m_FacingRight)
        {
            Flip();
        }
        else if (move < 0 && m_FacingRight)
        {
            Flip();
        }

        // Jump logic (keep as is, AddForce is good here)
        if (m_Grounded && jump)
        {
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    private void Flip()
    {
        m_FacingRight = !m_FacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool IsGrounded()
    {
        return m_Grounded;
    }

    public float GetFacingDirection()
    {
        return m_FacingRight ? 1f : -1f;
    }
}