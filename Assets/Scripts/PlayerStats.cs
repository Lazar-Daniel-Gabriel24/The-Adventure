using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerHealth playerHealth;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;

    [Header("Base Stats")]
    public int baseHealth = 100;
    public int baseDamage = 40;
    public float baseSpeed = 400f;

    [Header("Upgrade Percentages")]
    [Range(0f, 1f)] public float healthPercentPerPoint = 0.10f;
    [Range(0f, 1f)] public float damagePercentPerPoint = 0.25f;
    [Range(0f, 1f)] public float speedPercentPerPoint = 0.05f;

    [Header("Upgrade Points")]
    public int healthPoints = 0;
    public int damagePoints = 0;
    public int speedPoints = 0;

    [Header("XP and Level")]
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    public int availablePoints = 0;

    void Start()
    {
        ApplyUpgrades();
    }

    public void GainXP(int amount)
    {
        currentXP += amount;
        while (currentXP >= xpToNextLevel)
        {
            currentXP -= xpToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        availablePoints++;
        xpToNextLevel = Mathf.RoundToInt(xpToNextLevel * 1.5f);
        Debug.Log("Level Up! New level: " + level);
    }

    public bool TryAddHealthPoint()
    {
        if (availablePoints > 0)
        {
            healthPoints++;
            availablePoints--;
            ApplyUpgrades();
            return true;
        }
        return false;
    }

    public bool TryAddDamagePoint()
    {
        if (availablePoints > 0)
        {
            damagePoints++;
            availablePoints--;
            ApplyUpgrades();
            return true;
        }
        return false;
    }

    public bool TryAddSpeedPoint()
    {
        if (availablePoints > 0)
        {
            speedPoints++;
            availablePoints--;
            ApplyUpgrades();
            return true;
        }
        return false;
    }

    public void  ApplyUpgrades()
    {
        int upgradedHealth = Mathf.RoundToInt(baseHealth * (1f + healthPercentPerPoint * healthPoints));
        int upgradedDamage = Mathf.RoundToInt(baseDamage * (1f + damagePercentPerPoint * damagePoints));
        float upgradedSpeed = baseSpeed * (1f + speedPercentPerPoint * speedPoints);

        if (playerHealth != null)
        {
            playerHealth.maxHealth = upgradedHealth;
            playerHealth.currentHealth = upgradedHealth;
            playerHealth.healthBar.SetMaxHealth(upgradedHealth);
            playerHealth.healthBar.SetHealth(upgradedHealth);
        }

        if (playerCombat != null)
        {
            playerCombat.attackDamage = upgradedDamage;
        }

        if (playerMovement != null)
        {
            playerMovement.runSpeed = upgradedSpeed;
        }
    }
}
