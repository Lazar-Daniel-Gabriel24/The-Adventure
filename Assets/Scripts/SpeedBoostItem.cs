using UnityEngine;

public class SpeedBoostItem : Item
{
    public float boostAmount = 2f;
    public float duration = 5f;

    public override void UseItem()
    {
        PlayerMovement player = FindObjectOfType<PlayerMovement>();
        if (player != null)
        {
            player.StartCoroutine(player.SpeedBoost(boostAmount, duration));
            Debug.Log($"Speed boosted for {duration} seconds.");
        }

        Destroy(gameObject); // consumă itemul
    }
}
