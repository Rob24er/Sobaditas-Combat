using System;
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
        print("takehit");
        if (IsDead) return;

        // decidir si se bloquea
        bool blocked = isGuarding && hitbox.height != HitHeight.Low;

        if (blocked)
        {
            // registrar bloqueo
            lastBlockedTime = Time.time;

            // reproducir VFX de bloqueo
            if (vfx != null)
                vfx.PlayBlockVFX(hitPoint.position);

            var go = Instantiate(damageVFX, hitPoint.position, Quaternion.identity);
            go.GetComponent<TextMeshPro>().text = "Blocked!";
        }
        else
        {
            // registrar golpe recibido
            lastDamagedTime = Time.time;

            // aplicar da?o
            currentHealth -= hitbox.damage;
            var go = Instantiate(damageVFX, hitPoint.position, Quaternion.identity);
            go.GetComponent<TextMeshPro>().text = hitbox.damage.ToString();



            // reproducir VFX de golpe
            if (vfx != null)
                vfx.PlayHitVFX(hitPoint.position);

            // comprobar muerte
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                Debug.Log(name + " DEAD");
            }
            UpdateBar();

        }
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
