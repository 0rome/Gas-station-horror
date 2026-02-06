using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlSettings : MonoBehaviour
{
    [SerializeField] private ControlData controlData;
    [SerializeField] private Button saveButton;

    [Header("Buttons")]
    [SerializeField] private Button moveUpButton;
    [SerializeField] private Button moveDownButton;
    [SerializeField] private Button moveLeftButton;
    [SerializeField] private Button moveRightButton;
    [SerializeField] private Button flashlightButton;
    [SerializeField] private Button runButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text moveUpText;
    [SerializeField] private TMP_Text moveDownText;
    [SerializeField] private TMP_Text moveLeftText;
    [SerializeField] private TMP_Text moveRightText;
    [SerializeField] private TMP_Text flashlightText;
    [SerializeField] private TMP_Text runText;

    [Header("Mouse Settings")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private TMP_Text mouseSensitivityValueText;

    private Button waitingForButton;

    private void Start()
    {
        LoadSettings();

        UpdateTexts();

        saveButton.onClick.AddListener(SaveSettings);

        moveUpButton.onClick.AddListener(() => StartRebind(moveUpButton));
        moveDownButton.onClick.AddListener(() => StartRebind(moveDownButton));
        moveLeftButton.onClick.AddListener(() => StartRebind(moveLeftButton));
        moveRightButton.onClick.AddListener(() => StartRebind(moveRightButton));
        flashlightButton.onClick.AddListener(() => StartRebind(flashlightButton));
        runButton.onClick.AddListener(() => StartRebind(runButton));

        mouseSensitivitySlider.value = controlData.mouseSensitivity;
        mouseSensitivityValueText.text = mouseSensitivitySlider.value.ToString("F1");

        // Когда пользователь двигает слайдер
        mouseSensitivitySlider.onValueChanged.AddListener((value) =>
        {
            controlData.mouseSensitivity = value;
            mouseSensitivityValueText.text = value.ToString("F1");
        });
    }

    private void StartRebind(Button btn)
    {
        waitingForButton = btn;
        HighlightButton(btn, true);
    }

    private void Update()
    {
        if (waitingForButton == null) return;

        // Проверяем любую клавишу
        if (Input.anyKeyDown)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    // Меняем нужную клавишу
                    if (waitingForButton == moveUpButton) controlData.moveUpKey = key;
                    if (waitingForButton == moveDownButton) controlData.moveDownKey = key;
                    if (waitingForButton == moveLeftButton) controlData.moveLeftKey = key;
                    if (waitingForButton == moveRightButton) controlData.moveRightKey = key;
                    if (waitingForButton == flashlightButton) controlData.flashlightKey = key;
                    if (waitingForButton == runButton) controlData.runKey = key;

                    // Сбрасываем кнопку
                    HighlightButton(waitingForButton, false);
                    waitingForButton = null;
                    UpdateTexts();
                    break;
                }
            }
        }
    }


    private void UpdateTexts()
    {
        moveUpText.text = controlData.moveUpKey.ToString();
        moveDownText.text = controlData.moveDownKey.ToString();
        moveLeftText.text = controlData.moveLeftKey.ToString();
        moveRightText.text = controlData.moveRightKey.ToString();
        flashlightText.text = controlData.flashlightKey.ToString();
        runText.text = controlData.runKey.ToString();
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("MoveUpKey", (int)controlData.moveUpKey);
        PlayerPrefs.SetInt("MoveDownKey", (int)controlData.moveDownKey);
        PlayerPrefs.SetInt("MoveLeftKey", (int)controlData.moveLeftKey);
        PlayerPrefs.SetInt("MoveRightKey", (int)controlData.moveRightKey);
        PlayerPrefs.SetInt("FlashlightKey", (int)controlData.flashlightKey);
        PlayerPrefs.SetInt("RunKey", (int)controlData.runKey);

        PlayerPrefs.SetFloat("MouseSensitivity", controlData.mouseSensitivity);

        PlayerPrefs.Save();

        Debug.Log("Settings saved!");
    }
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey("MoveUpKey")) controlData.moveUpKey = (KeyCode)PlayerPrefs.GetInt("MoveUpKey");
        if (PlayerPrefs.HasKey("MoveDownKey")) controlData.moveDownKey = (KeyCode)PlayerPrefs.GetInt("MoveDownKey");
        if (PlayerPrefs.HasKey("MoveLeftKey")) controlData.moveLeftKey = (KeyCode)PlayerPrefs.GetInt("MoveLeftKey");
        if (PlayerPrefs.HasKey("MoveRightKey")) controlData.moveRightKey = (KeyCode)PlayerPrefs.GetInt("MoveRightKey");
        if (PlayerPrefs.HasKey("FlashlightKey")) controlData.flashlightKey = (KeyCode)PlayerPrefs.GetInt("FlashlightKey");
        if (PlayerPrefs.HasKey("RunKey")) controlData.runKey = (KeyCode)PlayerPrefs.GetInt("RunKey");

        if (PlayerPrefs.HasKey("MouseSensitivity"))
            controlData.mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");
    }

    private void HighlightButton(Button btn, bool state)
    {
        if (btn == null) return;

        TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
        if (text == null) return;

        if (state)
            text.text = "Press...";
        else
            UpdateTexts(); // вернёт нормальный текст
    }


}
