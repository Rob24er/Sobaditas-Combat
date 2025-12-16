using UnityEngine;

public class FighterVFX : MonoBehaviour
{
    public GameObject hitVFX;
    public GameObject blockVFX;
    public float lifeTime = 0.5f;

    public void PlayHitVFX(Vector3 position)
    {
        if (!hitVFX) return;

        GameObject vfx = Instantiate(hitVFX, position, Quaternion.identity);
        Destroy(vfx, lifeTime);
    }

    public void PlayBlockVFX(Vector3 position)
    {
        if (!blockVFX) return;

        GameObject vfx = Instantiate(blockVFX, position, Quaternion.identity);
        Destroy(vfx, lifeTime);
    }
}