using System;
using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public Scrollbar healthBar;
    public InGameTextVfx damageVFX;

    [Header("State")]
    public bool isGuarding;
    public bool isDead;

   
    public bool IsDead => isDead;

    [Header("Animation")]
    public Animator animator;

    [Header("VFX")]
    public FighterVFX vfx;
    public Transform hitPoint;

    [Header("Timers")]
    public float lastDamagedTime = -10f;
    public float lastBlockedTime = -10f;

    [Header("Death")]
    public float restartDelay = 5f;

    void Awake()
    {
        currentHealth = maxHealth;

        if (vfx == null)
            vfx = GetComponent<FighterVFX>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (hitPoint == null)
            hitPoint = transform;

        UpdateBar();
    }

    public void TakeHit(Hitbox hitbox)
    {
        if (isDead) return;

        bool blocked = isGuarding && hitbox.height != HitHeight.Low;

        if (blocked)
        {
            lastBlockedTime = Time.time;

            if (vfx != null)
                vfx.PlayBlockVFX(hitPoint.position);

            var go = Instantiate(damageVFX, hitPoint.position, Quaternion.identity);
            go.GetComponent<TextMeshPro>().text = "Blocked!";
        }
        else
        {
            lastDamagedTime = Time.time;

            currentHealth -= hitbox.damage;

            var go = Instantiate(damageVFX, hitPoint.position, Quaternion.identity);
            go.GetComponent<TextMeshPro>().text = hitbox.damage.ToString();

            if (vfx != null)
                vfx.PlayHitVFX(hitPoint.position);

            if (currentHealth <= 0)
            {
                Die();
            }

            UpdateBar();
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        currentHealth = 0;

        Debug.Log(name + " DEAD");

        if (animator != null)
        {
            animator.ResetTrigger("Die");
            animator.SetTrigger("Die");
        }

        StartCoroutine(RestartSceneAfterDelay());
    }

    IEnumerator RestartSceneAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateBar()
    {
        if (healthBar == null) return;

        float ratio = (float)currentHealth / (float)maxHealth;
        healthBar.size = Mathf.Clamp01(ratio);
    }

    public void MessageFix(string msg)
    {
        var go = Instantiate(damageVFX, hitPoint.position, Quaternion.identity);
        go.GetComponent<TextMeshPro>().text = msg;
    }
}