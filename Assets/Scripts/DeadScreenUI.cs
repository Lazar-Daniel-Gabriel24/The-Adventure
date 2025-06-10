using UnityEngine;
using UnityEngine.SceneManagement;

public class DeadScreenUI : MonoBehaviour
{
    [SerializeField] private GameObject deadScreenPanel;

    public void ShowDeadScreen()
    {
        if (deadScreenPanel != null)
        {
            Debug.Log("Dead screen activated.");
            deadScreenPanel.SetActive(true);
            Time.timeScale = 0f; // opțional
        }
        else
        {
            Debug.LogError("Dead screen panel is not assigned!");
        }
    }

    public void TryAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
