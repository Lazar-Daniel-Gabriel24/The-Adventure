using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Cinemachine;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;
    private HotbarController hotbarController;
    private PlayerStats playerStats; // Reference to PlayerStats

    // Use Awake to ensure references are set before Start on other scripts potentially uses them
    void Awake()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        // Find controllers and player stats. Add null checks.
        inventoryController = FindObjectOfType<InventoryController>();
        if (inventoryController == null)
        {
            Debug.LogError("SaveController: InventoryController not found in scene!");
        }

        hotbarController = FindObjectOfType<HotbarController>();
        if (hotbarController == null)
        {
            Debug.LogError("SaveController: HotbarController not found in scene!");
        }

        playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats == null)
        {
            Debug.LogError("SaveController: PlayerStats not found in scene!");
        }
    }

    void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        if (playerStats == null)
        {
            Debug.LogError("Cannot save game: PlayerStats is null.");
            return;
        }

        // Ensure playerHealth is not null before accessing its properties
        if (playerStats.playerHealth == null)
        {
            Debug.LogError("Cannot save game: PlayerHealth is null on PlayerStats.");
            return;
        }

        // Ensure inventoryController and hotbarController are not null
        if (inventoryController == null || hotbarController == null)
        {
            Debug.LogError("Cannot save game: InventoryController or HotbarController is null.");
            return;
        }

        SaveGame saveGame = new SaveGame
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            // Add a null check for CinemachineConfiner and its bounding shape
            mapBoundary = (FindAnyObjectByType<CinemachineConfiner>() != null && FindAnyObjectByType<CinemachineConfiner>().m_BoundingShape2D != null) ? FindAnyObjectByType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name : "DefaultMapBoundary", // Provide a default or handle appropriately
            inventorySaveData = inventoryController.GetInventoryItems(),
            hotbarSaveData = hotbarController.GetHotbarItems(),

            currentHealth = playerStats.playerHealth.currentHealth,
            maxHealth = playerStats.playerHealth.maxHealth,
            level = playerStats.level,
            currentXP = playerStats.currentXP,
            xpToNextLevel = playerStats.xpToNextLevel,
            damagePoints = playerStats.damagePoints,
            speedPoints = playerStats.speedPoints,
            healthPoints = playerStats.healthPoints,
            availablePoints = playerStats.availablePoints
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveGame));
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            SaveGame saveData = JsonUtility.FromJson<SaveGame>(File.ReadAllText(saveLocation));

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null)
            {
                Debug.LogError("LoadGame: Player object with tag 'Player' not found!");
                return;
            }
            player.transform.position = saveData.playerPosition;

            CinemachineConfiner confiner = FindObjectOfType<CinemachineConfiner>();
            if (confiner != null && GameObject.Find(saveData.mapBoundary) != null)
            {
                confiner.m_BoundingShape2D = GameObject.Find(saveData.mapBoundary).GetComponent<PolygonCollider2D>();
            }
            else
            {
                Debug.LogWarning("LoadGame: CinemachineConfiner or specified map boundary not found. Using current map boundary.");
            }

            // Null checks for controllers before using them
            if (inventoryController != null)
            {
                inventoryController.SetInventoryItems(saveData.inventorySaveData);
            }
            else
            {
                Debug.LogError("LoadGame: InventoryController is null, cannot set inventory items.");
            }

            if (hotbarController != null)
            {
                hotbarController.SetHotbarItems(saveData.hotbarSaveData);
            }
            else
            {
                Debug.LogError("LoadGame: HotbarController is null, cannot set hotbar items.");
            }

            // Restaurare statistici jucător
            if (playerStats == null)
            {
                playerStats = player.GetComponent<PlayerStats>();
                if (playerStats == null)
                {
                    Debug.LogError("LoadGame: PlayerStats component not found on player object!");
                    return;
                }
            }

            playerStats.level = saveData.level;
            playerStats.currentXP = saveData.currentXP;
            playerStats.xpToNextLevel = saveData.xpToNextLevel;
            playerStats.damagePoints = saveData.damagePoints;
            playerStats.speedPoints = saveData.speedPoints;
            playerStats.healthPoints = saveData.healthPoints;
            playerStats.availablePoints = saveData.availablePoints;

            playerStats.ApplyUpgrades(); // Aplică upgrade-urile

            // Ensure playerHealth exists before accessing it
            if (playerStats.playerHealth != null)
            {
                playerStats.playerHealth.currentHealth = saveData.currentHealth;
                playerStats.playerHealth.maxHealth = saveData.maxHealth;
                // Ensure healthBar exists before using it
                if (playerStats.playerHealth.healthBar != null)
                {
                    playerStats.playerHealth.healthBar.SetMaxHealth(saveData.maxHealth);
                    playerStats.playerHealth.healthBar.SetHealth(saveData.currentHealth);
                }
                else
                {
                    Debug.LogWarning("LoadGame: Player's HealthBar is null.");
                }
            }
            else
            {
                Debug.LogError("LoadGame: PlayerHealth component is null on PlayerStats.");
            }

            Debug.Log("Game Loaded!");
        }
        else
        {
            Debug.Log("No save file found. Creating a new game save.");
            // Only save if playerStats and its dependencies are guaranteed to be initialized
            // This is safer to call from MainMenuController.NewGame() or after player setup is complete
            SaveGame();
        }
    }
}