using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public EnemyMovement movement;
    public EnemyAttack attack;
    public PlayerHealth health;

    void Awake()
    {
        if (movement == null) movement = GetComponent<EnemyMovement>();
        if (attack == null) attack = GetComponent<EnemyAttack>();
        if (health == null) health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (health != null && health.IsDead) return;

        if (attack != null) attack.TickAI();
        if (movement != null) movement.TickMovement();
    }
}
