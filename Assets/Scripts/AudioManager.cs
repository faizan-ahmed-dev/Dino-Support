using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip cityMusic;
    public AudioClip dinoMusic;

    void Awake()
    {
        Instance = this;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayCityMusic()
    {
        if (cityMusic == null) return;
        musicSource.clip = cityMusic;
        musicSource.Play();
    }

    public void PlayDinoMusic()
    {
        if (dinoMusic == null) return;
        musicSource.clip = dinoMusic;
        musicSource.Play();
    }
}