using UnityEngine;

public class DamageBoostItem : Item
{
    public int bonusDamage = 20;
    public float duration = 5f;

    public override void UseItem()
    {
        PlayerCombat combat = FindObjectOfType<PlayerCombat>();
        if (combat != null)
        {
            combat.StartCoroutine(combat.DamageBoost(bonusDamage, duration));
            Debug.Log($"Damage boosted by {bonusDamage} for {duration} seconds.");
        }

        Destroy(gameObject); // consumă itemul
    }
}
