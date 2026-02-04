using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("UI")]
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle musicToggle;
    public Toggle sfxToggle;

    // PlayerPrefs keys
    const string MUSIC_KEY = "MusicVolume";
    const string SFX_KEY = "SFXVolume";
    const string MUSIC_MUTE_KEY = "MusicMute";
    const string SFX_MUTE_KEY = "SFXMute";

    // Volume range (dB)
    const float MIN_DB = -70f;
    const float MUSIC_MAX_DB = -30f; // 👈 nhạc nhỏ hơn để không rè
    const float SFX_MAX_DB = -20f;    // 👈 SFX to hơn nhạc chút

    void Awake()
    {
        // Lần đầu chơi → mặc định bật
        if (!PlayerPrefs.HasKey(MUSIC_MUTE_KEY))
            PlayerPrefs.SetInt(MUSIC_MUTE_KEY, 0);

        if (!PlayerPrefs.HasKey(SFX_MUTE_KEY))
            PlayerPrefs.SetInt(SFX_MUTE_KEY, 0);

        if (!PlayerPrefs.HasKey(MUSIC_KEY))
            PlayerPrefs.SetFloat(MUSIC_KEY, 0.5f);

        if (!PlayerPrefs.HasKey(SFX_KEY))
            PlayerPrefs.SetFloat(SFX_KEY, 0.5f);

        PlayerPrefs.Save();
    }

    void Start()
    {
        // Load value
        float musicVol = PlayerPrefs.GetFloat(MUSIC_KEY);
        float sfxVol = PlayerPrefs.GetFloat(SFX_KEY);

        bool musicMute = PlayerPrefs.GetInt(MUSIC_MUTE_KEY) == 1;
        bool sfxMute = PlayerPrefs.GetInt(SFX_MUTE_KEY) == 1;

        // Set UI (KHÔNG trigger event)
        musicSlider.value = musicVol;
        sfxSlider.value = sfxVol;

        musicToggle.SetIsOnWithoutNotify(!musicMute);
        sfxToggle.SetIsOnWithoutNotify(!sfxMute);

        ApplyMusic();
        ApplySFX();
    }

    // ================= MUSIC =================
    public void ToggleMusic()
    {
        bool isOn = musicToggle.isOn;

        PlayerPrefs.SetInt(MUSIC_MUTE_KEY, isOn ? 0 : 1);
        PlayerPrefs.Save();

        ApplyMusic();
    }

    public void SetMusic()
    {
        PlayerPrefs.SetFloat(MUSIC_KEY, musicSlider.value);
        PlayerPrefs.Save();

        ApplyMusic();
    }

    void ApplyMusic()
    {
        musicSlider.interactable = musicToggle.isOn;

        if (!musicToggle.isOn)
        {
            audioMixer.SetFloat("MusicVolume", -80f);
            return;
        }

        float db = Mathf.Lerp(MIN_DB, MUSIC_MAX_DB, musicSlider.value);
        audioMixer.SetFloat("MusicVolume", db);
    }

    // ================= SFX =================
    public void ToggleSFX()
    {
        bool isOn = sfxToggle.isOn;

        PlayerPrefs.SetInt(SFX_MUTE_KEY, isOn ? 0 : 1);
        PlayerPrefs.Save();

        ApplySFX();
    }

    public void SetSFX()
    {
        PlayerPrefs.SetFloat(SFX_KEY, sfxSlider.value);
        PlayerPrefs.Save();

        ApplySFX();
    }

    void ApplySFX()
    {
        sfxSlider.interactable = sfxToggle.isOn;

        if (!sfxToggle.isOn)
        {
            audioMixer.SetFloat("SFXVolume", -80f);
            return;
        }

        float db = Mathf.Lerp(MIN_DB, SFX_MAX_DB, sfxSlider.value);
        audioMixer.SetFloat("SFXVolume", db);
    }
}
