using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
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

    Coroutine attackTimeoutRoutine;

    bool lastAttackState;

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

    void Update()
    {
        if (isAttacking != lastAttackState)
        {
            Debug.Log("isAttacking = " + isAttacking);
            lastAttackState = isAttacking;
        }
    }

    public void TickCombat()
    {
        HandleGuard();
        HandleAttackInput();
    }

    void HandleGuard()
    {
        bool guardInput = Input.GetKey(KeyCode.Space);

        health.isGuarding = guardInput;
        animator.SetBool(hashIsGuarding, guardInput);
    }

    void HandleAttackInput()
    {
        if (health != null && health.IsDead) return;

        int height = 1;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            height = 0;
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            height = 2;

        int type = -1;
        if (Input.GetKeyDown(KeyCode.J)) type = 0;
        else if (Input.GetKeyDown(KeyCode.K)) type = 1;
        else if (Input.GetKeyDown(KeyCode.L)) type = 2;

        if (type == -1) return;

        animator.SetInteger(hashAttackType, type);
        animator.SetInteger(hashAttackHeight, height);
        animator.SetTrigger(hashAttack);

        isAttacking = true;

        // seguro por si el evento no llega
        if (attackTimeoutRoutine != null) StopCoroutine(attackTimeoutRoutine);
        attackTimeoutRoutine = StartCoroutine(AttackResetTimeout());
    }

    IEnumerator AttackResetTimeout()
    {
        //seguro
        yield return new WaitForSeconds(1.5f);

        if (isAttacking)
        {
            Debug.Log("Timeout de ataque -> reseteando isAttacking");
            isAttacking = false;

            if (hitboxLow != null) hitboxLow.DisableHit();
            if (hitboxMid != null) hitboxMid.DisableHit();
            if (hitboxHigh != null) hitboxHigh.DisableHit();
        }
    }


    public void HitboxOn(int height)
    {
        Debug.Log("HitboxOn llamado, altura " + height);
        SwitchHitbox(height, true);
    }

    public void HitboxOff(int height)
    {
        Debug.Log("HitboxOff llamado, altura " + height);
        SwitchHitbox(height, false);
    }

    void SwitchHitbox(int height, bool enable)
    {
        Hitbox target = null;

        if (height == 0) target = hitboxLow;
        else if (height == 1) target = hitboxMid;
        else if (height == 2) target = hitboxHigh;

        if (target == null) return;

        if (enable) target.EnableHit();
        else target.DisableHit();
    }

    public void AttackFinished()
    {
        Debug.Log("AttackFinished EVENT RECEIVED");

        isAttacking = false;

        if (attackTimeoutRoutine != null)
        {
            StopCoroutine(attackTimeoutRoutine);
            attackTimeoutRoutine = null;
        }

        if (hitboxLow != null) hitboxLow.DisableHit();
        if (hitboxMid != null) hitboxMid.DisableHit();
        if (hitboxHigh != null) hitboxHigh.DisableHit();
    }
}