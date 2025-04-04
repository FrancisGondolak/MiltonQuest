using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource louderSfxSource;

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClickSFX;

    //patrón Singleton, para asegurarse de que solo haya una instancia de AudioManager en todo el juego
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
    }

    private void Start()
    {
        PlayMusic(backgroundMusic);
    }

    //método para la reproducción de música
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip != clip)
        {
            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    //método para la reproducción de efectos de sonido a un volumen medio
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    //método para la reproducción de efectos de sonido a un volumen alto (para efectos de sonido que se oigan demasiado bajo)
    public void PlayLouderSFX(AudioClip clip)
    {
        if (clip != null)
        {
            louderSfxSource.PlayOneShot(clip);
        }
    }

    //método para el sonido que hacen los botones de los menús al hacer click sobre ellos
    public void PlayButtonClick()
    {
        PlayLouderSFX(buttonClickSFX);
    }
}
