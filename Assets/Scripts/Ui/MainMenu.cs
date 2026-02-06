using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private CanvasGroup MainButtons;
    [SerializeField] private CanvasGroup SettingsMenu;
    [SerializeField] private float fadeSpeed = 20f;

    private int menuIndex;

    private void Update()
    {
        if (menuIndex == 0)
        {
            // Плавное появление/исчезновение
            MainButtons.alpha = Mathf.Lerp(MainButtons.alpha, 1f, Time.deltaTime * fadeSpeed);
            SettingsMenu.alpha = Mathf.Lerp(SettingsMenu.alpha, 0f, Time.deltaTime * fadeSpeed);

            // Сделать кнопки интерактивными сразу, без привязки к alpha
            MainButtons.interactable = true;
            SettingsMenu.interactable = false;
        }
        else
        {
            MainButtons.alpha = Mathf.Lerp(MainButtons.alpha, 0f, Time.deltaTime * fadeSpeed);
            SettingsMenu.alpha = Mathf.Lerp(SettingsMenu.alpha, 1f, Time.deltaTime * fadeSpeed);

            MainButtons.interactable = false;
            SettingsMenu.interactable = true;
        }
    }

    public void PlayButton()
    {
        SceneLoader.Instance.LoadScene(1);
    }

    public void SettingsButton()
    {
        menuIndex = 1;
    }

    public void BackButton()
    {
        menuIndex = 0;
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
