using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    [Header("UI Reference")]
    public DeadScreenUI deadScreenUI;

    private PlayerAudioAdvanced playerAudio;
    private bool isDead = false;  // flag pentru stare mort

    void Start()
    {
        currentHealth = maxHealth;
        playerAudio = GetComponent<PlayerAudioAdvanced>();

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        else
        {
            Debug.LogError("HealthBar reference not set in PlayerHealth!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return; // dacă e mort, nu mai primește damage

        currentHealth -= damage;

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        playerAudio?.PlayHurtSound();

        if (currentHealth <= 0)
        {
            isDead = true;

            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerCombat>().enabled = false;

            Debug.Log("Player has died.");

            // oprește toate sunetele active înainte să dai play la sunetul de moarte
            playerAudio?.StopAllAudio();

            playerAudio?.PlayDeathSound();

            if (deadScreenUI != null)
            {
                deadScreenUI.ShowDeadScreen();
            }
            else
            {
                Debug.LogError("DeadScreenUI reference not set in PlayerHealth!");
            }
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return; // nu vindecă dacă e mort

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        playerAudio?.PlayHealSound();
    }
}
