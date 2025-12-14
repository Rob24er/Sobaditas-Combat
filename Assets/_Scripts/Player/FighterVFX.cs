using UnityEngine;

public class FighterVFX : MonoBehaviour
{
    public GameObject hitVFX;
    public GameObject blockVFX;

    public void PlayHitVFX(Vector3 position)
    {
        if (hitVFX)
            Instantiate(hitVFX, position, Quaternion.identity);
    }

    public void PlayBlockVFX(Vector3 position)
    {
        if (blockVFX)
            Instantiate(blockVFX, position, Quaternion.identity);
    }
}
