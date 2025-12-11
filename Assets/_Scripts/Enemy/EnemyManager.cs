using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public EnemyMovement movement;
    public EnemyAttack attack;
    public PlayerHealth health;

    [Header("Rangos")]
    public float closeRange = 1.2f;   // cuerpo a cuerpo
    public float farRange = 3f;     // distancia a partir de la cual se acerca sí o sí

    [Header("IA")]
    public float decisionCooldown = 0.6f;
    public LayerMask visionMask = ~0; 

    float lastDecisionTime;
    float currentMoveDirection = 0f;

    void Awake()
    {
        if (movement == null) movement = GetComponent<EnemyMovement>();
        if (attack == null) attack = GetComponent<EnemyAttack>();
        if (health == null) health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (health != null && health.IsDead)
        {
            if (movement != null) movement.Stop();
            if (attack != null) attack.SetGuard(false);
            return;
        }

        if (player == null || movement == null || attack == null) return;

        FacePlayer();

        if (attack.IsAttacking)
        {
            movement.Stop();
            return;
        }

        float dist = HorizontalDistanceToPlayer();
        bool seesPlayer = HasLineOfSight();

        if (Time.time < lastDecisionTime + decisionCooldown)
        {
            movement.Move(currentMoveDirection);
            return;
        }

        lastDecisionTime = Time.time;

        if (!seesPlayer)
        {
            //avanzar
            attack.SetGuard(false);
            DecideMoveTowardPlayer(+1);
            return;
        }

        // lejos del todo -> acercarse
        if (dist > farRange)
        {
            attack.SetGuard(false);
            DecideMoveTowardPlayer(+1);
        }
        // media distancia
        else if (dist > closeRange)
        {
            float r = Random.value;
            if (r < 0.7f)
            {
                attack.SetGuard(false);
                DecideMoveTowardPlayer(+1);
            }
            else
            {
                currentMoveDirection = 0f;
                movement.Stop();
                attack.SetGuard(true);
            }
        }
        // muy cerca -> atacar / bloquear / retroceder
        else
        {
            float r = Random.value;

            if (r < 0.6f)
            {
                // atacar
                currentMoveDirection = 0f;
                movement.Stop();
                attack.SetGuard(false);

                int height = Random.Range(0, 3);
                int type = Random.Range(0, 3); 

                attack.DoAttack(height, type);
            }
            else if (r < 1.2f)
            {
                // bloquear
                currentMoveDirection = 0f;
                movement.Stop();
                attack.SetGuard(true);
            }
            else
            {
                // retroceder
                attack.SetGuard(false);
                DecideMoveTowardPlayer(-1);
            }
        }
    }

    void DecideMoveTowardPlayer(float directionSign)
    {
        // directionSign: +1 acercarse, -1 alejarse
        currentMoveDirection = directionSign;
        movement.Move(currentMoveDirection);
    }

    float HorizontalDistanceToPlayer()
    {
        Vector3 a = transform.position;
        Vector3 b = player.position;
        a.y = b.y = 0f;
        return Vector3.Distance(a, b);
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

    bool HasLineOfSight()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 target = player.position + Vector3.up * 1f;
        Vector3 dir = target - origin;
        float dist = dir.magnitude;
        if (dist < 0.001f) return true;

        dir /= dist;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, dist, visionMask))
        {
            //visión
            if (hit.transform == player || hit.transform.IsChildOf(player))
                return true;

            return false;
        }

        return true; 
    }
}