using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenUI : MonoBehaviour
{
    public GameObject endScreenPanel;

    void Start()
    {
        if (endScreenPanel != null)
            endScreenPanel.SetActive(false);
    }

    public void ShowEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
            Time.timeScale = 0f; // oprește timpul în joc când apare end screen-ul
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

        // Dacă ești în Editor:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
