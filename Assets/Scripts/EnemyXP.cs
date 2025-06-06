using UnityEngine;

public class EnemyXP : MonoBehaviour
{
    public int xpReward = 25;

    public void Die()
    {
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.GainXP(xpReward);
        }

        
    }
}
