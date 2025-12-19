using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public EnemyMovement movement;
    public EnemyAttack attack;
    public PlayerHealth health;
    public Animator animator;

    [Header("Distances")]
    public float meleeRange = 2f;
    public float attackDistance = 1.2f;
    public float retreatDistance = 2.2f;

    [Header("Block")]
    public float blockDuration = 1.0f;

    [Header("AI Rhythm")]
    public Vector2 idleBetweenActions = new Vector2(0.5f, 0.8f);
    public float damageReactWindow = 0.1f;

    int hashXMovement;
    float lastXDir = 1f;
    Coroutine aiRoutine;

    void Awake()
    {
        movement ??= GetComponent<EnemyMovement>();
        attack ??= GetComponent<EnemyAttack>();
        health ??= GetComponent<PlayerHealth>();
        animator ??= GetComponent<Animator>();
    }

    void Start()
    {
        if (!player)
        {
            Debug.LogWarning($"{name} EnemyManager has no player assigned");
            enabled = false;
            return;
        }

        hashXMovement = Animator.StringToHash("XMovement");
        aiRoutine = StartCoroutine(AILoop());
    }

    void Update()
    {
        if (!health.IsDead)
            FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(dir.normalized);
    }


    IEnumerator AILoop()
    {
        while (!health.IsDead)
        {
            float dist = DistanceToPlayer();

            if (RecentlyDamaged())
            {
                YieldGuardOff();
                yield return RetreatToDistance(retreatDistance);
                yield return Idle();
                continue;
            }

            if (dist > meleeRange)
            {
                YieldGuardOff();
                yield return MoveToDistance(meleeRange);
                yield return Idle();
                continue;
            }

            yield return Idle();

            if (health.IsDead) break;
            dist = DistanceToPlayer();
            if (dist > meleeRange) continue;

            if (dist > attackDistance)
                yield return MidRangeDecision();
            else
                yield return CloseRangeDecision();
        }

        ShutdownAI();
    }


    IEnumerator MidRangeDecision()
    {
        int roll = Random.Range(0, 100);

        if (roll < 60)
        {
            YieldGuardOff();
            yield return MoveToDistance(attackDistance);
            yield return Idle();
        }
        else if (roll < 85)
        {
            yield return BlockSequence();
        }
        else
        {
            YieldGuardOff();
            yield return RetreatToDistance(retreatDistance);
        }
    }

    IEnumerator CloseRangeDecision()
    {
        int roll = Random.Range(0, 100);

        if (roll < 65)
            yield return AttackSequence();
        else if (roll < 90)
            yield return BlockSequence();
        else
        {
            YieldGuardOff();
            yield return RetreatToDistance(retreatDistance);
        }
    }

 
    IEnumerator AttackSequence()
    {
        yield return MoveToDistance(attackDistance);
        if (health.IsDead) yield break;

        yield return AttackOnce();
        if (health.IsDead) yield break;

        int post = Random.Range(0, 100);

        if (post < 40 && !RecentlyDamaged())
            yield return AttackOnce();
        else if (post < 80)
            yield return BlockSequence();
        else
        {
            YieldGuardOff();
            yield return RetreatToDistance(retreatDistance);
        }
    }

    IEnumerator AttackOnce()
    {
        if (health.IsDead) yield break;

        StopMovement();
        YieldGuardOff();

        attack.DoAttack(Random.Range(0, 3), Random.Range(0, 3));

        while (attack.IsAttacking && !health.IsDead)
            yield return null;

        yield return Idle();
    }

    IEnumerator BlockSequence()
    {
        StopMovement();
        attack.SetGuard(true);

        float blockStart = health.lastBlockedTime;
        float damageStart = health.lastDamagedTime;
        float end = Time.time + blockDuration;

        while (Time.time < end && !health.IsDead)
            yield return null;

        YieldGuardOff();
        if (health.IsDead) yield break;

        bool blocked = health.lastBlockedTime > blockStart;
        bool damaged = health.lastDamagedTime > damageStart;

        if (blocked)
            yield return AttackOnce();
        else if (damaged)
        {
            if (Random.value < 0.5f)
            {
                yield return MoveToDistance(attackDistance);
                yield return AttackOnce();
            }
            else
                yield return RetreatToDistance(retreatDistance);
        }
        else
            yield return RetreatToDistance(retreatDistance);
    }


    IEnumerator MoveToDistance(float desired)
    {
        while (!health.IsDead)
        {
            if (RecentlyDamaged()) yield break;

            float delta = DistanceToPlayer() - desired;

            if (Mathf.Abs(delta) < 0.05f)
                break;

            Move(delta > 0f ? 1f : -1f);
            yield return null;
        }

        StopMovement();
    }

    IEnumerator RetreatToDistance(float desired)
    {
 
        float startDist = DistanceToPlayer();
        float targetDist = Mathf.Max(startDist, desired);

        while (!health.IsDead)
        {
            float currentDist = DistanceToPlayer();

            if (currentDist >= targetDist - 0.05f)
                break;

            Move(-1f); 
            yield return null;
        }

        StopMovement();
    }

 
    bool RecentlyDamaged() =>
        Time.time - health.lastDamagedTime < damageReactWindow;

    float DistanceToPlayer()
    {
        Vector3 a = transform.position;
        Vector3 b = player.position;
        a.y = b.y = 0f;
        return Vector3.Distance(a, b);
    }

    void Move(float dir)
    {
        movement.Move(dir);
        UpdateXMovement(dir);
    }

    void StopMovement()
    {
        movement.Stop();
        UpdateXMovement(0f);
    }

    void YieldGuardOff() => attack.SetGuard(false);

    IEnumerator Idle()
    {
        StopMovement();
        yield return new WaitForSeconds(Random.Range(idleBetweenActions.x, idleBetweenActions.y));
    }

    void UpdateXMovement(float xMove)
    {
        if (Mathf.Approximately(xMove, 0f))
            xMove = lastXDir;
        else
            lastXDir = xMove;

        animator.SetFloat(hashXMovement, xMove);
    }

    void ShutdownAI()
    {
        StopAllCoroutines();
        StopMovement();
        YieldGuardOff();
    }
}