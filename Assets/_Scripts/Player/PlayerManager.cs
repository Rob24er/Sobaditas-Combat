using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerMovement movement;
    public PlayerAttack attack;
    public PlayerHealth health;

    void Awake()
    {
        //pillar los codigos del player
        if (movement == null) movement = GetComponent<PlayerMovement>();
        if (attack == null) attack = GetComponent<PlayerAttack>();
        if (health == null) health = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (health != null && health.IsDead) return;

        if (attack != null) attack.TickCombat();

        //si ataca no mov
        if (movement != null && (attack == null || !attack.IsAttacking))
        {
            movement.TickMovement();
        }
        else if (movement != null)
        {
            movement.animator.SetFloat("Speed", 0f);
        }
    }
}
