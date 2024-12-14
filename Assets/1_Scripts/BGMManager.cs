using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;
    public int CurrentPriority { get; set; } = 0;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
    }

    public void PlayBGM(AudioClip clip, float volume)
    {
        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return;
        }

        audioSource.volume = volume;

        StartCoroutine(SwitchMusic(clip, volume));
    }

    private System.Collections.IEnumerator SwitchMusic(AudioClip newClip, float volume)
    {
        yield return StartCoroutine(FadeOut());

        audioSource.clip = newClip;
        audioSource.volume = volume;
        audioSource.Play();
        Debug.Log("BGM Playing: " + newClip.name);

        yield return StartCoroutine(FadeIn());
    }

    public void StopBGM()
    {
        StartCoroutine(FadeOut());
    }

    private System.Collections.IEnumerator FadeOut()
    {
        for (float volume = 1; volume > 0; volume -= Time.deltaTime)
        {
            audioSource.volume = volume;
            yield return null;
        }
        audioSource.Stop();
    }

    private System.Collections.IEnumerator FadeIn()
    {
        for (float volume = 0; volume <= 1; volume += Time.deltaTime)
        {
            audioSource.volume = volume;
            yield return null;
        }
    }
}
