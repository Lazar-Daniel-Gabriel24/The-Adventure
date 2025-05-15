using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SaveGame 
{
    public Vector3 playerPosition;
    public string mapBoundary;
    public List<InventorySaveData> inventorySaveData;
}
