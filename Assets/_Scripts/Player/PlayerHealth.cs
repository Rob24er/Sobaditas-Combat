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

    public bool IsDead;

    void Start()
    {
        CurrentHealth = MaxHealth;
        UpdateHealthBar();
        IsDead = false;
    }

    public void TakeHit(Hitbox hit)
    {
        if (hit == null) return;

        if (isGuarding && canBlockHighAndMid && hit.height != HitHeight.Low)
        {
            Debug.Log("Block");
            return;
        }

        CurrentHealth -= hit.damage;
        if (CurrentHealth < 0) CurrentHealth = 0;

        Debug.Log("Health = " + CurrentHealth);

        UpdateHealthBar();

        if (CurrentHealth <= 0)
        {
            OnDeath();
            IsDead = true;
        }
    }

    public void UpdateHealthBar()
    {
        if (healthBar == null || MaxHealth <= 0) return;

        float ratio = (float)CurrentHealth / (float)MaxHealth;
        ratio = Mathf.Clamp01(ratio);

        healthBar.size = ratio;
    }

    public void OnDeath()
    {
        Debug.Log("F");
    }
}