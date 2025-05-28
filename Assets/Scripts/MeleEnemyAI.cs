using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MeleEnemyAi : MonoBehaviour
{
    public Transform target; // Jucătorul
    public float Speed = 200f;
    public float nextWaypointDistance = 1f;
    public float detectionRange = 10f;
    public float attackRange = 1f;

    public Transform enemyGFX;
    public Transform AttackPoint;
    public LayerMask playerLayer;

    [HideInInspector]
    public bool isDead = false;

    private int currentPatrolIndex = 0;
    private bool isPatrolling = true;

    private Path path;
    private int currentWaypoint = 0;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator animator;

    private float lastFlipX = 1f;

    public int attackDamage = 20;
    public float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    public Transform[] patrolPoints;

    // 🔻 Damage on collision
    private float lastHitTime = -999f;
    public float hitCooldown = 1f;
    public int contactDamage = 10;

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

            if (seeker.IsDone())
                seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
        else
        {
            isPatrolling = true;
            path = null;
            currentWaypoint = 0;
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

        if (isPatrolling)
        {
            Patrol();
        }
        else
        {
            FollowPlayer();
            TryAttack();
        }
    }

    void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        Vector2 direction = ((Vector2)patrolTarget.position - rb.position).normalized;
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
        if (Time.time < nextAttackTime) return;

        Collider2D hitPlayer = Physics2D.OverlapCircle(AttackPoint.position, attackRange, playerLayer);

        if (hitPlayer != null)
        {
            animator.SetTrigger("Attack");
            hitPlayer.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void FlipSprite(float moveX)
    {
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

    // 🔻 Damage player on contact
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            if (playerRb != null)
            {
                Vector2 knockbackDirection = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDirection * 10f, ForceMode2D.Impulse); // poți ajusta forța
            }
        }
    }


    private void OnDrawGizmosSelected()
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
    }

}
