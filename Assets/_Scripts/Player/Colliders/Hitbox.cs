using UnityEngine;
using System.Collections;
public enum HitHeight
{
    Low,   // 0
    Mid,   // 1
    High   // 2
}
public enum HitStrength
{
    Light,
    Medium,
    Heavy
}
public class Hitbox : MonoBehaviour
{
    public HitHeight height = HitHeight.Mid;
    public HitStrength strength = HitStrength.Light;
    public int damage = 10;
    public FighterVFX testVfx;
    public PlayerHealth ownerHealth;

    [Header("Hit VFX Offsets")]
    public Vector3 lowHitOffset;
    public Vector3 midHitOffset;
    public Vector3 highHitOffset;

    Collider col;

    bool hasHitSomeone;
    void Awake()
    {
        col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
            col.enabled = false;
        }

        if (ownerHealth == null)
        {
            ownerHealth = GetComponentInParent<PlayerHealth>();
        }
    }

    //Da�o activar desactivar
    public void EnableHit()
    {
        hasHitSomeone = false;
        if (col != null) col.enabled = true;
    }

    public void DisableHit()
    {
        if (col != null) col.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (hasHitSomeone) return;

        Hurtbox hurt = other.GetComponent<Hurtbox>();
        if (hurt == null)
            hurt = other.GetComponentInParent<Hurtbox>();

        if (hurt == null) return;
        if (hurt.health == null) return;

        if (hurt.health == ownerHealth) return;

        Debug.Log(
        "[HITBOX] " + name + " HIT -> Damage: " + damage + " Height: " + height + " Target: " + hurt.name);

        hasHitSomeone = true;
        if (testVfx != null)
        {
            Vector3 hitPos = transform.position;

            switch (height)
            {
                case HitHeight.Low:
                    hitPos += lowHitOffset;
                    break;

                case HitHeight.Mid:
                    hitPos += midHitOffset;
                    break;

                case HitHeight.High:
                    hitPos += highHitOffset;
                    break;
            }

            testVfx.PlayHitVFX(hitPos);
        }
        hurt.health.TakeHit(this);
        
    }
}
