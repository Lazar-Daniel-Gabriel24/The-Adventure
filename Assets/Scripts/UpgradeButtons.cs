using UnityEngine;

public class UpgradeButtons : MonoBehaviour
{
    public PlayerStats playerStats;

    public void UpgradeSTR()
    {
        playerStats.TryAddDamagePoint();
    }

    public void UpgradeAGI()
    {
        playerStats.TryAddSpeedPoint();
    }

    public void UpgradeDUR()
    {
        playerStats.TryAddHealthPoint();
    }
}
