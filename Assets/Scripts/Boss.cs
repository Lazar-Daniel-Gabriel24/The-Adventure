using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;
    private bool isFacingRight = false;

    void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void LookAtPlayer()
    {
        if (player == null) return;

        float direction = player.position.x - transform.position.x;
        if (direction > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (direction < 0 && isFacingRight)
        {
            Flip();
        }
    }
}
