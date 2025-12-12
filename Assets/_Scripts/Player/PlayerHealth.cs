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

    SkinnedMeshRenderer[] skins;
    Material[] mats;
    Color[] original;
    Coroutine flashCo;

    public float lastDamagedTime = 0f;
    public float lastBlockedTime = 0f;
    void Awake()
    {
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

        UpdateBar();
    }
    public void TakeHit(Hitbox hit)
    {
        if (IsDead) return;

        if (IsBlockedByGuard(hit))
        {
            lastBlockedTime = Time.time;
            return;
        }

        currentHealth -= hit.damage;
        lastDamagedTime = Time.time;

        UpdateBar();

        if (flashCo != null) StopCoroutine(flashCo);
        flashCo = StartCoroutine(FlashRed());

        if (currentHealth <= 0)
            gameObject.SetActive(false);
    }

    bool IsBlockedByGuard(Hitbox hit)
    {
        if (!isGuarding) return false;

        if (hit.height == HitHeight.Low) return false;

        if (!canBlockHighAndMid) return false;

        return hit.height == HitHeight.Mid || hit.height == HitHeight.High;
    }

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

    void UpdateBar()
    {
        if (healthBar == null) return;

        float ratio = (float)currentHealth / (float)maxHealth;
        healthBar.size = Mathf.Clamp01(ratio);
    }
}