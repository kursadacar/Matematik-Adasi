using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Dropdown qualitySetting;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider masterVolumeSlider;
    public void SetVolumeLevel(float value)
    {
        PlayerPrefs.SetFloat("MasterVolume", value);
        GameData.masterVolume = value;
        AudioListener.volume = value;
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        SoundDatabase.Instance.SetVolume(value);
        GameData.musicVolume = value;
    }

    public void ChangeQuality(Dropdown dropdown)
    {
        PlayerPrefs.SetInt("Quality", dropdown.value);
        QualitySettings.SetQualityLevel(dropdown.value);
    }

    private void Start()
    {
        if (GameData.SaveData.PlayingFirstTime)
        {
            PlayerPrefs.SetInt("Quality", 0);
            PlayerPrefs.SetFloat("MasterVolume", 1f);
            PlayerPrefs.SetFloat("MusicVolume", 1f);
        }
        QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality"));
        GameData.masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        GameData.musicVolume = PlayerPrefs.GetFloat("MusicVolume");

        qualitySetting.value = PlayerPrefs.GetInt("Quality");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    }
}
