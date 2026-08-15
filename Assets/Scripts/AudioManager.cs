using UnityEngine;

// One AudioManager lives in the scene the whole time (like GameManager).
// Anything can call AudioManager.Instance.PlaySFX(clip) or PlayMusic(clip).
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Drag the two AudioSource components from THIS object")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music tracks")]
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