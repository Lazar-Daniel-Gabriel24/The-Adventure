using UnityEngine;

public class VolumeLoader : MonoBehaviour
{
    void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.ApplySavedVolumes();
        }
    }
}
