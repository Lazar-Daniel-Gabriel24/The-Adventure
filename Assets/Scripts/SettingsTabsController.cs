using UnityEngine;

public class SettingsTabsController : MonoBehaviour
{
    public GameObject controlsPanel; // PanelKeybindsPanel
    public GameObject volumePanel;

    void Start()
    {
        ShowControls(); // C�nd intri, porne?te cu Controls
    }

    public void ShowControls()
    {
        controlsPanel.SetActive(true);
        volumePanel.SetActive(false);
    }

    public void ShowVolume()
    {
        controlsPanel.SetActive(false);
        volumePanel.SetActive(true);
    }
}
