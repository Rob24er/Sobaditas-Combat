using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Bloqueo")]
    public bool canBlockHighAndMid = true;
    [HideInInspector] public bool isGuarding;

    public bool IsDead => currentHealth <= 0;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeHit(Hitbox hit)
    {
        if (hit == null || IsDead) return;

        bool blockedNow = false;

        if (isGuarding && canBlockHighAndMid)
        {
            if (hit.height == HitHeight.Mid || hit.height == HitHeight.High)
            {
                blockedNow = true;
            }
        }

        if (blockedNow)
        {
            Debug.Log("block");
            return;
        }

        currentHealth -= hit.damage;

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        Debug.Log("F");
    }
}