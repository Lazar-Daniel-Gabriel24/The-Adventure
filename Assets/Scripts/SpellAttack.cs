using UnityEngine;

public class SpellAttack : MonoBehaviour
{
    public int damage = 20;
    public float knockbackForce = 6f;
    public float lifetime = 2f;
    public float activationDelay = 0.5f;

    private bool hasDamagedPlayer = false;
    private Collider2D spellCollider;

    void Start()
    {
        spellCollider = GetComponent<Collider2D>();
        spellCollider.enabled = false;
        Invoke(nameof(ActivateCollider), activationDelay);
        Destroy(gameObject, lifetime);
    }

    void ActivateCollider()
    {
        spellCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasDamagedPlayer) return;

        if (collision.CompareTag("Player"))
        {
            PlayerHealth ph = collision.GetComponent<PlayerHealth>();
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (ph != null)
                ph.TakeDamage(damage);

            if (rb != null)
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            }

            hasDamagedPlayer = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(transform.position, col.bounds.size);
        }
    }
}
