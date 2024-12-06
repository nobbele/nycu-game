using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    public AudioClip mainStageMusic;
    public AudioClip bossMusic;

    private AudioSource audioSource;

    public static MusicManager Instance { get; private set; }

    void Start()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        PlayStageMusic();
    }

    public void PlayStageMusic() => PlayClip(mainStageMusic);
    public void PlayBossMusic() => PlayClip(bossMusic);

    private void PlayClip(AudioClip clip) 
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
