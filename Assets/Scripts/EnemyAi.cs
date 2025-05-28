using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyAi : MonoBehaviour
{
    public Transform target; // Jucătorul
    public float speed = 200f;
    public float nextWaypointDistance = 1f;
    public float detectionRange = 10f;

    public Transform enemyGFX;
    public Transform[] patrolPoints;

    private int currentPatrolIndex = 0;
    private bool isPatrolling = true;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

    private Seeker seeker;
    private Rigidbody2D rb;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    void UpdatePath()
    {
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
        if (isPatrolling)
        {
            Patrol();
        }
        else
        {
            FollowPlayer();
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform patrolTarget = patrolPoints[currentPatrolIndex];
        Vector2 direction = ((Vector2)patrolTarget.position - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        FlipSprite(force.x);

        float distance = Vector2.Distance(rb.position, patrolTarget.position);
        if (distance < nextWaypointDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    void FollowPlayer()
    {
        if (path == null || currentWaypoint >= path.vectorPath.Count) return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;
        rb.AddForce(force);

        FlipSprite(force.x);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void FlipSprite(float moveX)
    {
        if (moveX >= 0.01f)
            enemyGFX.localScale = new Vector3(-1f, 1f, 1f);
        else if (moveX <= -0.01f)
            enemyGFX.localScale = new Vector3(1f, 1f, 1f);
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
    }
}
