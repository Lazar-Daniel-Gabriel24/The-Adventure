using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleEnemyStats : MonoBehaviour
{
    public Animator animator;
    public MeleEnemyAi enemyAi;  // Referință către scriptul MeleEnemyAi (setează în Inspector)

    public int maxHealth = 100;
    int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
            return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Enemy died");

        // Acordă XP jucătorului
        EnemyXP enemyXP = GetComponent<EnemyXP>();
        if (enemyXP != null)
        {
            enemyXP.Die();
        }

        animator.SetBool("IsDead", true);
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        if (enemyAi != null)
        {
            enemyAi.isDead = true;
        }

        Invoke("DisableEnemy", 1.5f);
    }


    void DisableEnemy()
    {
        gameObject.SetActive(false);
    }
}
