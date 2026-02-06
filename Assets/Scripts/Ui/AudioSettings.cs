using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;

    private void Start()
    {
        // Загружаем сохранённые значения (если нет — ставим 1)
        float masterSaved = PlayerPrefs.GetFloat("MasterVolumeValue", 1f);
        float musicSaved = PlayerPrefs.GetFloat("MusicVolumeValue", 1f);

        // Устанавливаем слайдеры
        masterSlider.value = masterSaved;
        musicSlider.value = musicSaved;

        // Применяем громкость сразу
        SetMasterVolume(masterSaved);
        SetMusicVolume(musicSaved);

        // Подписываемся на изменение
        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void SetMasterVolume(float value)
    {
        // Перевод в децибелы
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);

        // Сохраняем значение
        PlayerPrefs.SetFloat("MasterVolumeValue", value);
    }

    public void SetMusicVolume(float value)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);

        PlayerPrefs.SetFloat("MusicVolumeValue", value);
    }
}
