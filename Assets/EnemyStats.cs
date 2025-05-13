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

        // Activează animația de moarte
        animator.SetBool("IsDead", true);

        // Dezactivează coliziunea și acest script
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        // După 1.5 secunde, dezactivează GameObject-ul complet
        Invoke("DisableEnemy", 1.5f);
    }

    void DisableEnemy()
    {
        gameObject.SetActive(false);
    }
}
