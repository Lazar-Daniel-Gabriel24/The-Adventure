using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    public void NewGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "saveData.json");

        // Ștergem fișierul de salvare dacă există
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Fișierul saveData.json a fost șters.");
        }

        // Opțional: dacă SaveController salvează automat în Start(), atunci nu trebuie să faci nimic în plus

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
