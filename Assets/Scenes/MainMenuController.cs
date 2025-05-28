using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        // Datele inițiale (poți adapta după nevoie)
        string initialSaveData = "{\"playerPosition\":{\"x\":-17.53,\"y\":6.09,\"z\":-0.16},\"mapBoundary\":\"Room1\",\"inventorySaveData\":[],\"hotbarSaveData\":[]}";

        // Resetează fișierul cu datele inițiale
        File.WriteAllText(savePath, initialSaveData);
        Debug.Log("Fișierul saveData.json a fost resetat cu datele inițiale.");

        // Încarcă scena jocului
        SceneManager.LoadScene("SampleScene"); // înlocuiește cu numele scenei tale
    }

    public void LoadGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        if (File.Exists(savePath))
        {
            SceneManager.LoadScene("SampleScene"); // SaveController va face Load automat
        }
        else
        {
            Debug.LogWarning("Nu există date salvate!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game"); // pentru editor
    }
}
