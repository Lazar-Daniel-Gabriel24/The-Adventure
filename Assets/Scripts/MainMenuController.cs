using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;

    public void NewGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        string savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        SaveGame initialSaveData = new SaveGame
        {
            playerPosition = new Vector3(-17.53f, 6.09f, -0.16f),
            mapBoundary = "Room1",
            inventorySaveData = new List<InventorySaveData>(),
            hotbarSaveData = new List<InventorySaveData>(),
            currentHealth = 100,
            maxHealth = 100,
            level = 1,
            currentXP = 0,
            xpToNextLevel = 100,
            damagePoints = 0,
            speedPoints = 0,
            healthPoints = 0,
            availablePoints = 0
        };

        File.WriteAllText(savePath, JsonUtility.ToJson(initialSaveData));
        Debug.Log("Fișierul saveData.json a fost resetat cu datele inițiale.");

        SceneManager.LoadScene("SampleScene");
    }

    public void LoadGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        if (File.Exists(savePath))
        {
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            Debug.LogWarning("Nu există date salvate!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }

    public void OpenSettings()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
}
