using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour
{
    public Animator animator;
    public PlayerHealth health;

    public Hitbox hitboxLow;
    public Hitbox hitboxMid;
    public Hitbox hitboxHigh;

    int hashAttackType;
    int hashAttackHeight;
    int hashAttack;
    int hashIsGuarding;

    bool isAttacking;
    public bool IsAttacking => isAttacking;

    int currentAttackHeight = 1;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (health == null) health = GetComponent<PlayerHealth>();

        hashAttackType = Animator.StringToHash("AttackType");
        hashAttackHeight = Animator.StringToHash("AttackHeight");
        hashAttack = Animator.StringToHash("Attack");
        hashIsGuarding = Animator.StringToHash("IsGuarding");

        if (hitboxLow != null) hitboxLow.ownerHealth = health;
        if (hitboxMid != null) hitboxMid.ownerHealth = health;
        if (hitboxHigh != null) hitboxHigh.ownerHealth = health;
    }

    //ataque
    public void DoAttack(int height, int type)
    {
        if (isAttacking) return;
        if (health != null && health.IsDead) return;

        currentAttackHeight = height;

        animator.SetInteger(hashAttackType, type);
        animator.SetInteger(hashAttackHeight, height);
        animator.SetTrigger(hashAttack);

        isAttacking = true;
    }

    //Guard
    public void SetGuard(bool value)
    {
        if (health != null)
            health.isGuarding = value;

        if (animator != null)
            animator.SetBool(hashIsGuarding, value);
    }
    //HIT box on y off
    public void HitboxOn()
    {
        SwitchHitbox(currentAttackHeight, true);
    }

    public void HitboxOff()
    {
        SwitchHitbox(currentAttackHeight, false);
    }

    void SwitchHitbox(int height, bool enable)
{
    Debug.Log("SwitchHitbox altura " + height + " enable " + enable);

    Hitbox target = null;

    if (height == 0) target = hitboxLow;
    else if (height == 1) target = hitboxMid;
    else if (height == 2) target = hitboxHigh;

    if (target == null) return;

    if (height == 0) target.height = HitHeight.Low;
    else if (height == 1) target.height = HitHeight.Mid;
    else if (height == 2) target.height = HitHeight.High;

    if (enable) target.EnableHit();
    else target.DisableHit();
}


    //Evento dejar de atacar
    public void AttackFinished()
    {
        isAttacking = false;

        if (hitboxLow != null) hitboxLow.DisableHit();
        if (hitboxMid != null) hitboxMid.DisableHit();
        if (hitboxHigh != null) hitboxHigh.DisableHit();
    }
}