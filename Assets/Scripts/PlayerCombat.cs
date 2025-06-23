using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public Animator animator;

    public Transform AttackPoint;
    public LayerMask enemyLayers;

    public float attackRange = 0.5f;
    public int attackDamage = 40;
    public float attackRate = 2f;
    public float knockbackForce = 7f; // Knockback aplicat inamicului
    private Coroutine damageBoostRoutine;
    float nextAttackTime = 0f;
    private int baseAttackDamage;
    void Start()
    {
        baseAttackDamage = attackDamage;
    }
    public IEnumerator DamageBoost(int bonus, float duration)
    {
        attackDamage += bonus;

        yield return new WaitForSeconds(duration);

        attackDamage = baseAttackDamage;
    }

    void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            if (FindObjectOfType<KeybindSettings>().GetKeyDown("Attack"))
            {
                Attack();
                nextAttackTime = Time.time + 1f / attackRate;
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(AttackPoint.position, attackRange, enemyLayers);
        foreach (Collider2D enemy in hitEnemies)
        {
            Vector2 knockbackDir = (enemy.transform.position - transform.position).normalized;

            // Damage la MeleEnemy
            MeleEnemyStats meleEnemy = enemy.GetComponent<MeleEnemyStats>();
            if (meleEnemy != null)
            {
                meleEnemy.TakeDamage(attackDamage);
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                continue;
            }

            // Damage la Enemy normal
            EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(attackDamage);
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                continue;
            }

            // 🔥 Damage la Boss
            BossHealth bossHealth = enemy.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(attackDamage);
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                continue;
            }

            Debug.LogWarning($"Obiectul {enemy.name} nu are componentă cunoscută pentru damage!");
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (AttackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackPoint.position, attackRange);
    }
}
