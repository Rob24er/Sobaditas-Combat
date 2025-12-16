using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public Scrollbar healthBar;

    public bool isGuarding;
    public bool canBlockHighAndMid = true;

    public bool IsDead => currentHealth <= 0;

<<<<<<< Updated upstream
    SkinnedMeshRenderer[] skins;
    Material[] mats;
    Color[] original;
    Coroutine flashCo;

    public float lastDamagedTime = 0f;
    public float lastBlockedTime = 0f;

    [Header("VFX")]
    public SFX_Char sfx_Char;
=======
    [Header("VFX")]
    public FighterVFX vfx;
    public Transform hitPoint; //efectos

    [Header("Timers")]
    public float lastDamagedTime = -10f; 
    public float lastBlockedTime = -10f; 
>>>>>>> Stashed changes

    void Awake()
    {
        if (sfx_Char == null) sfx_Char = GetComponent<SFX_Char>();
        if (currentHealth <= 0) currentHealth = maxHealth;

        skins = GetComponentsInChildren<SkinnedMeshRenderer>(true);

        mats = new Material[skins.Length];
        original = new Color[skins.Length];

        for (int i = 0; i < skins.Length; i++)
        {
            mats[i] = skins[i].material;

            if (mats[i] != null && mats[i].HasProperty("_BaseColor"))
                original[i] = mats[i].GetColor("_BaseColor");
            else if (mats[i] != null && mats[i].HasProperty("_Color"))
                original[i] = mats[i].color;
        }

<<<<<<< Updated upstream
=======
        if (hitPoint == null)
            hitPoint = transform;
>>>>>>> Stashed changes
        UpdateBar();
    }

    //Recibir daño
    public void TakeHit(Hitbox hit)
    {
        if (IsDead) return;

        if (IsBlockedByGuard(hit))
        {
            lastBlockedTime = Time.time;
            if (sfx_Char != null) sfx_Char.PlayBlock();
            return;
        }

        currentHealth -= hit.damage;
        if (sfx_Char != null) sfx_Char.PlayHit();
        lastDamagedTime = Time.time;

        UpdateBar();

        if (flashCo != null) StopCoroutine(flashCo);
        flashCo = StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            if (sfx_Char != null) sfx_Char.PlayDeath();
            StartCoroutine(DisableAfter(0.2f));
        }
        
    }

    //Bloqueo
    bool IsBlockedByGuard(Hitbox hit)
    {
        if (!isGuarding) return false;

        if (hit.height == HitHeight.Low) return false;

        if (!canBlockHighAndMid) return false;

        return hit.height == HitHeight.Mid || hit.height == HitHeight.High;
    }

    //Color rojo
    IEnumerator FlashRed()
    {
        SetColor(Color.red);
        yield return new WaitForSeconds(0.25f);
        RestoreColor();
    }

    void SetColor(Color c)
    {
        for (int i = 0; i < mats.Length; i++)
        {
            var m = mats[i];
            if (m == null) continue;

            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            else if (m.HasProperty("_Color")) m.color = c;
        }
    }

    void RestoreColor()
    {
        for (int i = 0; i < mats.Length; i++)
        {
            var m = mats[i];
            if (m == null) continue;

            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", original[i]);
            else if (m.HasProperty("_Color")) m.color = original[i];
        }
    }

    //Barra HP UI
    void UpdateBar()
    {
        if (healthBar == null) return;

        float ratio = (float)currentHealth / (float)maxHealth;
        healthBar.size = Mathf.Clamp01(ratio);
    }

    IEnumerator DisableAfter(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
    }
}