using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [Header("UI")]
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TMP_Dropdown qualityDropdown;
    public Toggle vSyncToggle;
    public Slider fpsSlider;
    public TextMeshProUGUI fpsValueText;

    private Resolution[] resolutions;

    private void Start()
    {
        LoadResolutions();   // 1) Загружаем список разрешений
        ApplySavedSettings(); // 2) Применяем сохранённые параметры

        LoadQuality();
        LoadVSync();
        LoadFullscreen();
        LoadFPSLimit();
    }


    // ======================
    // APPLY SAVED SETTINGS
    // ======================
    void ApplySavedSettings()
    {
        // Разрешение
        if (PlayerPrefs.HasKey("resolution"))
        {
            int index = PlayerPrefs.GetInt("resolution");
            if (index >= 0 && index < Screen.resolutions.Length)
            {
                SetResolution(index);
            }
        }

        // Фулскрин
        Screen.fullScreen = PlayerPrefs.GetInt("fullscreen", Screen.fullScreen ? 1 : 0) == 1;

        // Качество
        int quality = PlayerPrefs.GetInt("quality", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(quality, true);

        // VSync
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("vsync", QualitySettings.vSyncCount);

        // FPS Limit
        int fps = PlayerPrefs.GetInt("fps", Application.targetFrameRate > 0 ? Application.targetFrameRate : 144);
        Application.targetFrameRate = fps;
    }

    // ======================
    // SAVE SETTINGS BUTTON
    // ======================
    public void SaveSettings()
    {
        PlayerPrefs.SetInt("resolution", resolutionDropdown.value);
        PlayerPrefs.SetInt("fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("quality", qualityDropdown.value);
        PlayerPrefs.SetInt("vsync", vSyncToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt("fps", Mathf.RoundToInt(fpsSlider.value));

        PlayerPrefs.Save();
    }

    // ============================
    // RESOLUTION
    // ============================
    void LoadResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            uint num = resolutions[i].refreshRateRatio.numerator;
            uint den = resolutions[i].refreshRateRatio.denominator;

            string hzText = (num == 0 || den == 0) ? "??" : ((float)num / den).ToString("F0");

            string op = $"{resolutions[i].width} x {resolutions[i].height} ({hzText}Hz)";
            options.Add(op);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        // Загрузка сохранённого
        resolutionDropdown.value = PlayerPrefs.GetInt("resolution", currentIndex);

        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    void SetResolution(int index)
    {
        Resolution res = resolutions[index];

        var rr = new RefreshRate
        {
            numerator = (uint)res.refreshRateRatio.numerator,
            denominator = (uint)res.refreshRateRatio.denominator
        };

        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode, rr);
    }

    // ============================
    // FULLSCREEN
    // ============================
    void LoadFullscreen()
    {
        fullscreenToggle.isOn = Screen.fullScreen;

        fullscreenToggle.onValueChanged.AddListener(toggle =>
        {
            Screen.fullScreen = toggle;
        });
    }

    // ============================
    // QUALITY
    // ============================
    void LoadQuality()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        qualityDropdown.onValueChanged.AddListener(index =>
        {
            QualitySettings.SetQualityLevel(index, true);
        });
    }

    // ============================
    // VSYNC
    // ============================
    void LoadVSync()
    {
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

        vSyncToggle.onValueChanged.AddListener(on =>
        {
            QualitySettings.vSyncCount = on ? 1 : 0;
        });
    }

    // ============================
    // FPS LIMITER
    // ============================
    void LoadFPSLimit()
    {
        fpsSlider.value = Application.targetFrameRate > 0 ? Application.targetFrameRate : 144;
        fpsValueText.text = fpsSlider.value + " FPS";

        fpsSlider.onValueChanged.AddListener(value =>
        {
            int fps = Mathf.RoundToInt(value);
            fpsValueText.text = fps + " FPS";
            Application.targetFrameRate = fps;
        });
    }
}
