using UnityEngine;

public class SFX_Char : MonoBehaviour
{
    public AudioSource source;

    [Header("SFX")]
    public AudioClip[] attackSFX;
    public AudioClip[] hitSFX;
    public AudioClip[] blockSFX;
    public AudioClip[] deathSFX;

    void Awake()
    {
        if (source == null) source = GetComponent<AudioSource>();
        if (source == null) source = gameObject.AddComponent<AudioSource>();
    }

    public void PlayAttack()
    {
        PlayRandom(attackSFX);
    }

    public void PlayHit()
    {
        PlayRandom(hitSFX);
    }

    public void PlayBlock()
    {
        PlayRandom(blockSFX);
    }

    public void PlayDeath()
    {
        PlayRandom(deathSFX);
    }

    void PlayRandom(AudioClip[] clips)
    {
        if (clips == null) return;
        if (clips.Length == 0) return;

        int i = Random.Range(0, clips.Length);
        AudioClip c = clips[i];
        if (c == null) return;

        source.PlayOneShot(c);
    }
}
