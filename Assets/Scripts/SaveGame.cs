using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveGame
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public List<InventorySaveData> inventorySaveData;
    public List<InventorySaveData> hotbarSaveData;

    public int currentHealth;
    public int maxHealth;
    public int level;
    public int currentXP;
    public int xpToNextLevel;
    public int damagePoints;
    public int speedPoints;
    public int healthPoints;
    public int availablePoints;

    public bool hasDoubleJump;
}
