using UnityEngine;

public class DoubleJumpItem : MonoBehaviour
{
    private string uniqueID;

    private void Awake()
    {
        uniqueID = gameObject.scene.name + "_" + gameObject.name;

        // Dacă a fost deja luat, îl ascunde
        if (PlayerPrefs.GetInt(uniqueID, 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                player.EnableDoubleJump();

                // Marchez itemul ca luat
                PlayerPrefs.SetInt(uniqueID, 1);
                PlayerPrefs.Save();
            }

            Destroy(gameObject);
        }
    }
}
