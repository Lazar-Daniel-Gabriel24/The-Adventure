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

    // Referință pentru end screen UI (poate fi un panel în canvas)
    public GameObject endScreenPanel;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);  // Ascunde end screen la început
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

        EnemyXP enemyXP = GetComponent<EnemyXP>();
        if (enemyXP != null)
        {
            enemyXP.Die(); // oferă XP jucătorului
        }

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        animator.SetTrigger("Die");
        // În loc să distrugi instantaneu, așteaptă animația și apoi afișează end screen
        Invoke(nameof(ShowEndScreen), 2f);  // 2 secunde cât durează animația de moarte
    }

    private void ShowEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }
        // Poți opri timpul aici, dacă vrei
        Time.timeScale = 0f;
        // Distruge bossul după ce afișezi end screen (sau poți face asta după ce jucătorul apasă ceva)
        Destroy(gameObject);
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
