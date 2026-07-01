using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Brillo")]
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Image brightnessOverlay;
    [SerializeField, Range(0f, 1f)] private float maxDarkness = 0.65f;

    [Header("Volumen de música")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string musicVolumeParameter = "MusicVolume";

    [Header("Pantalla")]
    [SerializeField] private Toggle fullscreenToggle;

    private const string BrightnessKey = "Brightness";
    private const string MusicVolumeKey = "MusicVolume";
    private const string FullscreenKey = "Fullscreen";

    private void Start()
    {
        LoadSettings();

        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.AddListener(SetBrightness);

        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);

        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);

        if(!brightnessOverlay.gameObject.activeSelf)
            brightnessOverlay.gameObject.SetActive(true);
    }

    private void LoadSettings()
    {
        float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, 1f);
        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 0.5f);
        bool savedFullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        if (brightnessSlider != null)
            brightnessSlider.value = savedBrightness;

        if (musicVolumeSlider != null)
            musicVolumeSlider.value = savedMusicVolume;

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = savedFullscreen;

        SetBrightness(savedBrightness);
        SetMusicVolume(savedMusicVolume);
        SetFullscreen(savedFullscreen);
    }

    public void SetBrightness(float value)
    {
        value = Mathf.Clamp01(value);

        if (brightnessOverlay != null)
        {
            float darknessAlpha = (1f - value) * maxDarkness;
            Color color = brightnessOverlay.color;
            color.a = darknessAlpha;
            brightnessOverlay.color = color;
        }

        PlayerPrefs.SetFloat(BrightnessKey, value);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);

        if (audioMixer != null)
        {
            float volumeDb;

            if (value <= 0.0001f)
                volumeDb = -80f;
            else
                volumeDb = Mathf.Log10(value) * 20f;

            audioMixer.SetFloat(musicVolumeParameter, volumeDb);
        }

        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen
            ? FullScreenMode.FullScreenWindow
            : FullScreenMode.Windowed;

        Screen.fullScreen = isFullscreen;

        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();
    }
}