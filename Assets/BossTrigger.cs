using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Animator anim = GetComponentInParent<Animator>();
            anim.SetBool("IsChasing", true);
        }
    }
}
