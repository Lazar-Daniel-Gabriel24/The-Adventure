using UnityEngine;

public class Boss_Run : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float attackRange = 5f;

    Transform player;
    Rigidbody2D rb;
    Boss boss;
    BossHealth bossHealth;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = animator.GetComponent<Rigidbody2D>();
        boss = animator.GetComponent<Boss>();
        bossHealth = animator.GetComponent<BossHealth>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null || bossHealth == null) return;

        boss.LookAtPlayer();

        float distanceToPlayer = Vector2.Distance(player.position, rb.position);

        // Spell din depărtare când e enrage
        if (bossHealth.IsEnraged() && Time.time - bossHealth.LastSpellTime() >= bossHealth.spellCooldown)
        {
            if (Random.value < 0.3f) // 30% șansă de spell
            {
                animator.SetTrigger("CastSpell");
                return;
            }
        }

        // Dacă e aproape, atacă
        if (distanceToPlayer <= attackRange)
        {
            animator.SetBool("InAttackRange", true);
            animator.SetTrigger("AttackTrigger");
        }
        else
        {
            animator.SetBool("InAttackRange", false);
            Vector2 target = new Vector2(player.position.x, rb.position.y);
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("AttackTrigger");
        animator.ResetTrigger("CastSpell");
        animator.SetBool("InAttackRange", false);
    }
}
