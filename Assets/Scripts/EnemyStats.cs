using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public Animator animator;

    public int maxHealth = 100;
    int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
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

        Invoke("DisableEnemy", 0.5f);
    }


    void DisableEnemy()
    {
        gameObject.SetActive(false);
    }
}
