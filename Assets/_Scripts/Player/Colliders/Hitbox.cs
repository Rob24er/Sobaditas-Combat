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

    public PlayerHealth ownerHealth;

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

    //Daño activar desactivar
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

        hurt.health.TakeHit(this);
    }
}
