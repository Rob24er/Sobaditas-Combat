using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int MaxHealth = 100;
    public int CurrentHealth;

    [Header("UI")]
    public Scrollbar healthBar;

    public bool isGuarding;
    public bool canBlockHighAndMid = true;

    [HideInInspector] public float lastBlockedTime = -999f;
    [HideInInspector] public float lastDamagedTime = -999f;

    public bool IsDead => CurrentHealth <= 0;

    void Start()
    {
        CurrentHealth = MaxHealth;
        UpdateHealthBar();
    }

    public void TakeHit(Hitbox hit)
    {
        if (hit == null || IsDead) return;

        Debug.Log(name + " recibe intento de golpe altura " + hit.height);

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
            Debug.Log(name + " bloquea golpe altura " + hit.height);
            lastBlockedTime = Time.time;
            return;
        }

        CurrentHealth -= hit.damage;
        if (CurrentHealth < 0) CurrentHealth = 0;

        Debug.Log(name + " recibe " + hit.damage + " daño. Vida " + CurrentHealth);
        lastDamagedTime = Time.time;

        UpdateHealthBar();

        if (CurrentHealth <= 0)
        {
            OnDeath();
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar == null || MaxHealth <= 0) return;

        float ratio = (float)CurrentHealth / (float)MaxHealth;
        ratio = Mathf.Clamp01(ratio);

        healthBar.size = ratio;
    }

    void OnDeath()
    {
        Debug.Log(name + " ha muerto");
    }
}