using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int health = 500;
    public GameObject deathEffect;

    private Animator animator;
    private bool isEnraged = false;
    private bool isDead = false;
    private bool isVulnerable = true;

    public GameObject spellPrefab;
    public Transform target;
    public float spellYOffset = 2f;

    private bool hasCastSpell = false;
    private float lastSpellTime = -10f;
    public float spellCooldown = 3f;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    public void TakeDamage(int damage)
    {
        if (!isVulnerable || isDead) return;

        health -= damage;

        if (!isEnraged && health <= 250)
        {
            isEnraged = true;
            isVulnerable = false;
            animator.SetBool("IsEnraged", true);
            animator.SetTrigger("CastSpell");
            return;
        }

        if (health <= 0)
        {
            Die();
        }
        else
        {
            isVulnerable = false;
            animator.SetTrigger("Hurt");
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        // XP-ul este acordat aici
        EnemyXP enemyXP = GetComponent<EnemyXP>();
        if (enemyXP != null)
        {
            enemyXP.Die(); // oferă XP jucătorului
        }

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        animator.SetTrigger("Die");
        Destroy(gameObject, 2f);
    }


    public void CastSpell()
    {
        if (hasCastSpell || isDead || Time.time - lastSpellTime < spellCooldown) return;

        hasCastSpell = true;
        lastSpellTime = Time.time;

        if (target == null) return;

        Vector3 spawnPosition = target.position + new Vector3(0, spellYOffset, 0);
        Instantiate(spellPrefab, spawnPosition, Quaternion.identity);
    }

    public void SetVulnerableTrue()
    {
        isVulnerable = true;
        hasCastSpell = false;
    }

    // Metode publice folosite în Boss_Run
    public bool IsEnraged() => isEnraged;
    public float LastSpellTime() => lastSpellTime;
}
