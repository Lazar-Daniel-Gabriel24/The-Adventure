using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitToMainMenu : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu"; // schimbă dacă ai alt nume la scena de meniu

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
