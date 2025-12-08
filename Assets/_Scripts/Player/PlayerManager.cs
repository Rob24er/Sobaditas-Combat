using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerAttack attack;
    public PlayerHealth health;

    void Awake()
    {
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (attack == null) attack = GetComponent<PlayerAttack>();
        if (health == null) health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (health != null && health.IsDead) return;

        if (movement != null) movement.TickMovement();
        if (attack != null) attack.TickCombat();
    }
}
