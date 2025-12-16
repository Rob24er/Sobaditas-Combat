using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public Scrollbar healthBar;
    public InGameTextVfx damageVFX;
   
    
    //Hitstunt
    public Animator animator;
    


    [Header("State")]
    public bool isGuarding;
    public bool IsDead => currentHealth <= 0;

    [Header("VFX")]
    public FighterVFX vfx;
    public Transform hitPoint; // posici?n donde aparecer?n los efectos

    [Header("Timers")]
    public float lastDamagedTime = -10f; // momento del ?ltimo golpe recibido
    public float lastBlockedTime = -10f; // momento del ?ltimo bloqueo

    void Awake()
    {
        currentHealth = maxHealth;

        if (vfx == null)
            vfx = GetComponent<FighterVFX>();

        if (hitPoint == null)
            hitPoint = transform; // si no hay HitPoint asignado, usar pivot
        UpdateBar();
    }

    public void TakeHit(Hitbox hitbox)
    {
        if (IsDead) return;

        bool blocked = isGuarding && hitbox.height >= HitHeight.Mid;

        if (blocked)
        {
            lastBlockedTime = Time.time;

            if (vfx != null)
                vfx.PlayBlockVFX(hitPoint.position);

            var go = Instantiate(damageVFX, hitPoint.position, Quaternion.identity);
            go.GetComponent<TextMeshPro>().text = "Blocked!";

            return;
        }

        lastDamagedTime = Time.time;
        currentHealth -= hitbox.damage;
        currentHealth = Mathf.Max(currentHealth, 0);

        var dmg = Instantiate(damageVFX, hitPoint.position, Quaternion.identity);
        dmg.GetComponent<TextMeshPro>().text = hitbox.damage.ToString();

        if (vfx != null)
            vfx.PlayHitVFX(hitPoint.position);

        UpdateBar();

        if (animator != null)
            animator.SetTrigger("hitY");

        if (currentHealth <= 0)
        {
            if (animator != null)
                animator.SetTrigger("isDead");

            Invoke(nameof(RestartScene), 5f);
        }
    }
    void UpdateBar()
    {
        if (healthBar == null) return;

        float ratio = (float)currentHealth / (float)maxHealth;
        healthBar.size = Mathf.Clamp01(ratio);
    }
    void RestartScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}
