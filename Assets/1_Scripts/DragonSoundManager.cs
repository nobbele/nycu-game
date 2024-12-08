using UnityEngine;

public class DragonSoundManager : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip biteSound;
    public AudioClip breathSound;
    public AudioClip flameSound;
    public AudioClip hitSound;
    public AudioClip screamSound;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBiteSound()
    {
        PlaySound(biteSound);
    }

    public void PlayBreathSound()
    {
        PlaySound(breathSound);
    }

    public void PlayFlameSound()
    {
        PlaySound(flameSound);
    }

    public void PlayHitSound()
    {
        PlaySound(hitSound);
    }

    public void PlayScreamSound()
    {
        PlaySound(screamSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
