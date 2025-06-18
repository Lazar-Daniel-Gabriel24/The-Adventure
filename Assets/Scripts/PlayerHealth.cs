using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    [Header("UI Reference")]
    public DeadScreenUI deadScreenUI;

    void Start()
    {
        currentHealth = maxHealth;
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
        currentHealth -= damage;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {

            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerCombat>().enabled = false;


            Debug.Log("Player has died.");
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
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }
}
