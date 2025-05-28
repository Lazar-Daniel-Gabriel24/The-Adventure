using UnityEngine;

public class HealingItem : Item
{
    public int healAmount = 30;

    public override void UseItem()
    {
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
            Debug.Log($"Healed {healAmount} HP.");
        }

        Destroy(gameObject); // consumă itemul după utilizare
    }
}
