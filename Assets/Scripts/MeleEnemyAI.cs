using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MeleEnemyAi : MonoBehaviour
{
    [Header("Target & Movement")]
    public Transform target;
    public float Speed = 200f;
    public float maxSpeed = 2f;
    public float nextWaypointDistance = 1f;
    public float detectionRange = 10f;
    public float attackRange = 1f;

    [Header("Visual & Effects")]
    public Transform enemyGFX;
    public Transform AttackPoint;
    public LayerMask playerLayer;

    [Header("Combat Settings")]
    public int attackDamage = 20;
    public float attackCooldown = 1f;

    [Header("Patrolling")]
    public Transform[] patrolPoints;

    [Header("Contact Damage")]
    public float hitCooldown = 1f;
    public int contactDamage = 10;

    [Header("Ground Check")]
    public Transform groundCheckLeft;
    public Transform groundCheckRight;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    [HideInInspector]
    public bool isDead = false;

    private bool isKnockedback = false;
    private float knockbackDuration = 0.3f;
    private float knockbackTimer = 0f;

    private bool isAttacking = false;
    private float nextAttackTime = 0f;
    private float lastFlipX = 1f;
    private int currentPatrolIndex = 0;
    private bool isPatrolling = true;
    private Path path;
    private int currentWaypoint = 0;
    private float lastHitTime = -999f;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;

    private float lostPlayerTimer = 0f;
    private float lostPlayerThreshold = 2f; // timp până când inamicul renunță la urmărire

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
        if (isDead) return;

        float distanceToTarget = Vector2.Distance(rb.position, target.position);

        if (distanceToTarget <= detectionRange)
        {
            isPatrolling = false;
            lostPlayerTimer = 0f;

            if (seeker.IsDone())
                seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
        else
        {
            lostPlayerTimer += Time.deltaTime;

            if (lostPlayerTimer >= lostPlayerThreshold)
            {
                isPatrolling = true;
                path = null;
                currentWaypoint = 0;
            }
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            animator.SetFloat("Speed", 0f);
            rb.velocity = Vector2.zero;
            return;
        }

        if (isKnockedback)
        {
            knockbackTimer -= Time.fixedDeltaTime;
            if (knockbackTimer <= 0f)
                isKnockedback = false;

            animator.SetFloat("Speed", 0f);
            return;
        }

        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        if (!IsGrounded())
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        float distanceToPlayer = Vector2.Distance(rb.position, target.position);

        if (isPatrolling)
        {
            Patrol();
        }
        else if (distanceToPlayer <= attackRange)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            TryAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            FollowPlayer();
            TryAttack();
        }
        else
        {
            // Dacă jucătorul e prea departe, să revină la patrulare
            isPatrolling = true;
            path = null;
            currentWaypoint = 0;
            Patrol();
        }

        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        Vector2 direction = ((Vector2)patrolTarget.position - rb.position).normalized;

        float horizontalMove = direction.x;

        // Verifică marginea
        bool groundAhead = horizontalMove > 0 ?
            Physics2D.OverlapCircle(groundCheckRight.position, groundCheckRadius, groundLayer) :
            Physics2D.OverlapCircle(groundCheckLeft.position, groundCheckRadius, groundLayer);

        if (!groundAhead)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            return;
        }

        Vector2 force = direction * Speed * Time.deltaTime;
        rb.AddForce(force);

        FlipSprite(rb.velocity.x);

        float distance = Vector2.Distance(rb.position, patrolTarget.position);
        if (distance < nextWaypointDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    void FollowPlayer()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        float distanceToTarget = Vector2.Distance(rb.position, target.position);
        if (distanceToTarget <= attackRange)
        {
            rb.velocity = Vector2.zero;
            animator.SetFloat("Speed", 0f);
            return;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * Speed * Time.deltaTime;

        rb.AddForce(force);

        FlipSprite(rb.velocity.x);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        animator.SetFloat("Speed", rb.velocity.magnitude);
    }

    void TryAttack()
    {
        if (Time.time < nextAttackTime || isAttacking) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(AttackPoint.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            animator.SetTrigger("Attack");
            nextAttackTime = Time.time + attackCooldown;
            isAttacking = true;
        }
        else
        {
            animator.ResetTrigger("Attack");
        }
    }

    public void DealDamage()
    {
        if (isDead) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(AttackPoint.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    void FlipSprite(float moveX)
    {
        if (isKnockedback || isAttacking) return;

        if (moveX > 0.01f && lastFlipX != -1f)
        {
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
            lastFlipX = -1f;
        }
        else if (moveX < -0.01f && lastFlipX != 1f)
        {
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
            lastFlipX = 1f;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead || isAttacking) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(contactDamage);
            }

            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                rb.AddForce(-knockbackDirection * 0.1f, ForceMode2D.Impulse);

                isKnockedback = true;
                knockbackTimer = knockbackDuration;
            }
        }
    }

    bool IsGrounded()
    {
        bool groundedLeft = Physics2D.OverlapCircle(groundCheckLeft.position, groundCheckRadius, groundLayer);
        bool groundedRight = Physics2D.OverlapCircle(groundCheckRight.position, groundCheckRadius, groundLayer);
        return groundedLeft && groundedRight;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (patrolPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (var point in patrolPoints)
            {
                if (point != null)
                    Gizmos.DrawSphere(point.position, 0.2f);
            }
        }

        if (AttackPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
        }

        if (groundCheckLeft != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckLeft.position, groundCheckRadius);
        }

        if (groundCheckRight != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheckRight.position, groundCheckRadius);
        }
    }
}
