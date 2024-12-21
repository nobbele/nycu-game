using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    public AudioSource songAudioSource;
    public float Duration = 30f;

    private float timer;
    
    // Start is called before the first frame update
    void Start()
    {
        timer = Duration;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SceneManager.LoadScene("MainMenuScene");
        }

        var d = Mathf.Clamp01((timer - 1) / (Duration - 1));
        songAudioSource.volume = 1 - Mathf.Pow(1 - d, 3);
    }
}
