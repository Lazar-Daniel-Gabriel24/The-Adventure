using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Text UI")]
    public TextMeshProUGUI strText;
    public TextMeshProUGUI agiText;
    public TextMeshProUGUI durText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI xpText;
    public TextMeshProUGUI pointsText;

    [Header("Buttons")]
    public Button btnUpgradeSTR;
    public Button btnUpgradeAGI;
    public Button btnUpgradeDUR;

    [Header("References")]
    public PlayerStats playerStats;

    void Start()
    {
        btnUpgradeSTR.onClick.AddListener(UpgradeSTR);
        btnUpgradeAGI.onClick.AddListener(UpgradeAGI);
        btnUpgradeDUR.onClick.AddListener(UpgradeDUR);

        UpdateStats();
    }

    void Update()
    {
        UpdateStats();
    }

    void UpgradeSTR()
    {
        if (playerStats.TryAddDamagePoint())
            UpdateStats();
    }

    void UpgradeAGI()
    {
        if (playerStats.TryAddSpeedPoint())
            UpdateStats();
    }

    void UpgradeDUR()
    {
        if (playerStats.TryAddHealthPoint())
            UpdateStats();
    }

    void UpdateStats()
    {
        strText.text = "STR: " + playerStats.damagePoints;
        agiText.text = "AGI: " + playerStats.speedPoints;
        durText.text = "DUR: " + playerStats.healthPoints;

        levelText.text = "LV: " + playerStats.level;
        xpText.text = "XP: " + playerStats.currentXP + " / " + playerStats.xpToNextLevel;
        pointsText.text = "Points: " + playerStats.availablePoints;
    }
}
