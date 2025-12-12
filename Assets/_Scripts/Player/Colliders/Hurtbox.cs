using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public PlayerHealth health;

    void Awake()
    {
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }
}
