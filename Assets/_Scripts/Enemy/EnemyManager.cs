using UnityEngine;
using System.Collections;

public class EnemyManager : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public EnemyMovement movement;
    public EnemyAttack attack;
    public PlayerHealth health;
    public Animator animator;

    [Header("Distancias")]
    public float meleeRange = 2f;
    public float attackDistance = 1.2f;
    public float retreatDistance = 2.2f;

    int hashXMovement;
    float lastXDir = 1f;

    [Header("Bloqueo")]
    public float blockDuration = 1.0f;

    [Header("Ritmo IA")]
    public Vector2 idleBetweenActions = new Vector2(0.5f, 0.8f);

    public float damageReactWindow = 0.1f;

    void Awake()
    {
        if (movement == null) movement = GetComponent<EnemyMovement>();
        if (attack == null) attack = GetComponent<EnemyAttack>();
        if (health == null) health = GetComponent<PlayerHealth>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("EnemyManager no tiene player asignado");
            enabled = false;
            return;
        }

        hashXMovement = Animator.StringToHash("XMovement");
        StartCoroutine(AILoop());
    }

    void Update()
    {
        if (player != null && !health.IsDead)
            FacePlayer();
    }

    void FacePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(dir.normalized);
        }
    }

    // AI Main Loop
    IEnumerator AILoop()
    {
        while (!health.IsDead)
        {
            float dist = HorizontalDistanceToPlayer();

            if (Time.time - health.lastDamagedTime < damageReactWindow)
            {
                attack.SetGuard(false);
                yield return RetreatToDistance(retreatDistance);
                yield return SmallIdle();
                continue;
            }

            if (dist > meleeRange + 0.1f)
            {
                attack.SetGuard(false);
                yield return MoveToDistance(meleeRange);
                yield return SmallIdle();
                continue;
            }

            yield return SmallIdle();

            dist = HorizontalDistanceToPlayer();
            if (health.IsDead) break;
            if (dist > meleeRange + 0.1f) continue;

            if (dist > attackDistance + 0.1f)
            {
                int roll = Random.Range(0, 100);

                if (roll < 60)
                {
                    attack.SetGuard(false);
                    yield return MoveToDistance(attackDistance);
                    yield return SmallIdle();
                }
                else if (roll < 85)
                {
                    yield return BlockSequence();
                }
                else
                {
                    attack.SetGuard(false);
                    yield return RetreatToDistance(retreatDistance);
                }
            }
            else
            {
                int roll = Random.Range(0, 100);

                if (roll < 65)
                {
                    yield return AttackSequence();
                }
                else if (roll < 90)
                {
                    yield return BlockSequence();
                }
                else
                {
                    attack.SetGuard(false);
                    yield return RetreatToDistance(retreatDistance);
                }
            }
        }

        movement.Stop();
        UpdateXMovement(0f);
        attack.SetGuard(false);
    }

    IEnumerator SmallIdle()
    {
        float t = Random.Range(idleBetweenActions.x, idleBetweenActions.y);
        movement.Stop();
        UpdateXMovement(0f);
        yield return new WaitForSeconds(t);
    }

    IEnumerator MoveToDistance(float desiredDistance)
    {
        if (health.IsDead) yield break;

        while (!health.IsDead)
        {
            if (Time.time - health.lastDamagedTime < damageReactWindow)
                yield break;

            float dist = HorizontalDistanceToPlayer();
            float delta = dist - desiredDistance;

            if (Mathf.Abs(delta) < 0.05f)
            {
                movement.Stop();
                UpdateXMovement(0f);
                yield break;
            }

            float dirSign = delta > 0f ? 1f : -1f;
            movement.Move(dirSign);
            UpdateXMovement(dirSign);

            yield return null;
        }

        movement.Stop();
        UpdateXMovement(0f);
    }

    IEnumerator RetreatToDistance(float desiredDistance)
    {
        if (health.IsDead) yield break;

        while (!health.IsDead)
        {
            float dist = HorizontalDistanceToPlayer();
            float delta = dist - desiredDistance;

            if (Mathf.Abs(delta) < 0.05f)
            {
                movement.Stop();
                UpdateXMovement(0f);
                yield break;
            }

            float dirSign = dist < desiredDistance ? -1f : 1f;
            movement.Move(dirSign);
            UpdateXMovement(dirSign);

            yield return null;
        }

        movement.Stop();
        UpdateXMovement(0f);
    }

    IEnumerator AttackSequence()
    {
        if (health.IsDead) yield break;

        yield return MoveToDistance(attackDistance);
        if (health.IsDead) yield break;

        yield return AttackOnce();
        if (health.IsDead) yield break;

        int post = Random.Range(0, 100);

        if (post < 40)
        {
            if (Time.time - health.lastDamagedTime >= damageReactWindow)
                yield return AttackOnce();
        }
        else if (post < 80)
        {
            yield return BlockSequence();
        }
        else
        {
            attack.SetGuard(false);
            yield return RetreatToDistance(retreatDistance);
        }
    }

    IEnumerator AttackOnce()
    {
        if (health.IsDead) yield break;

        movement.Stop();
        UpdateXMovement(0f);
        attack.SetGuard(false);

        int height = Random.Range(0, 3);
        int type = Random.Range(0, 3);

        attack.DoAttack(height, type);

        while (attack.IsAttacking && !health.IsDead)
        {
            yield return null;
        }

        yield return SmallIdle();
    }

    IEnumerator BlockSequence()
    {
        if (health.IsDead) yield break;

        movement.Stop();
        UpdateXMovement(0f);
        attack.SetGuard(true);

        float startBlockedTime = health.lastBlockedTime;
        float startDamagedTime = health.lastDamagedTime;
        float endTime = Time.time + blockDuration;

        while (Time.time < endTime && !health.IsDead)
        {
            yield return null;
        }

        attack.SetGuard(false);
        if (health.IsDead) yield break;

        bool blockedSomething = health.lastBlockedTime > startBlockedTime;
        bool tookDamage = health.lastDamagedTime > startDamagedTime;

        if (blockedSomething)
        {
            yield return AttackOnce();
        }
        else if (tookDamage)
        {
            int choice = Random.Range(0, 2);

            if (choice == 0)
            {
                yield return MoveToDistance(attackDistance);
                yield return AttackOnce();
            }
            else
            {
                yield return RetreatToDistance(retreatDistance);
            }
        }
        else
        {
            yield return RetreatToDistance(retreatDistance);
        }
    }

    float HorizontalDistanceToPlayer()
    {
        Vector3 a = transform.position;
        Vector3 b = player.position;
        a.y = b.y = 0f;
        return Vector3.Distance(a, b);
    }

    // AI-based XMovement (no input)
    void UpdateXMovement(float xMove)
    {
        if (Mathf.Approximately(xMove, 0f))
            xMove = lastXDir;
        else
            lastXDir = xMove;

        animator.SetFloat(hashXMovement, xMove);
    }
}