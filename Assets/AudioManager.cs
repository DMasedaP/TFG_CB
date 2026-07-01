using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Source")]
    [SerializeField] private AudioSource musicSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParameter = "MusicVolume";

    [Header("Música")]
    [SerializeField] private AudioClip defaultMusicClip;
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private bool loopMusic = true;

    private const string MusicVolumeKey = "MusicVolume";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.loop = loopMusic;

        LoadMusicVolume();
    }

    private void Start()
    {
        if (playOnStart)
        {
            PlayDefaultMusic();
        }
    }

    public void PlayDefaultMusic()
    {
        if (defaultMusicClip == null)
        {
            Debug.LogWarning("No hay música asignada en el AudioManager.");
            return;
        }

        PlayMusic(defaultMusicClip);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);

        float volumeDb;

        if (value <= 0.0001f)
            volumeDb = -80f;
        else
            volumeDb = Mathf.Log10(value) * 20f;

        if (audioMixer != null)
            audioMixer.SetFloat(musicVolumeParameter, volumeDb);

        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
    }

    private void LoadMusicVolume()
    {
        float savedVolume = GetMusicVolume();
        SetMusicVolume(savedVolume);
    }
}