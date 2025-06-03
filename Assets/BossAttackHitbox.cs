using UnityEngine;

public class BossAttackHitbox : MonoBehaviour
{
    public int damage = 10;
    public float knockbackForce = 10f;

    private bool hasDealtDamage = false;

    private void OnEnable()
    {
        // Resetăm flagul când hitbox-ul este activat
        hasDealtDamage = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDealtDamage) return;

        Debug.Log("Boss hitbox triggered: " + collision.name);

        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();

            if (playerHealth != null)
                playerHealth.TakeDamage(damage);

            if (playerRb != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                playerRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }

            // marchez că a fost dat damage-ul
            hasDealtDamage = true;
        }
    }
}
