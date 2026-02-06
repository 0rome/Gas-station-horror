using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MiniMenu : MonoBehaviour
{
    [SerializeField] private GameObject MenuUi;
    [SerializeField] private GameObject SettingsUi;

    public bool blockMenu { get; set; } = false;

    private void Update()
    {
        if (blockMenu)
            return;

        if (Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex != 0)
        {
            if (MenuUi.activeSelf)
                StartCoroutine(HideMenuAndCursor());
            else if(!MenuUi.activeSelf && SettingsUi.activeSelf)
                BackButton();
            else
                ShowMenu();
        }
    }

    private void ShowMenu()
    {
        MenuUi.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private IEnumerator HideMenuAndCursor()
    {
        // Проверяем EventSystem
        var es = EventSystem.current;
        if (es != null)
            es.SetSelectedGameObject(null);

        // Деактивируем меню
        MenuUi.SetActive(false);

        // Несколько кадров подряд принудительно скрываем курсор
        for (int i = 0; i < 3; i++)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            yield return null;
        }

        // Сбрасываем Input axes
        Input.ResetInputAxes();

        // Если есть EventSystem — временно отключаем и включаем
        if (es != null)
        {
            es.enabled = false;
            yield return null;
            es.enabled = true;
        }
    }


    public void ResumeButton()
    {
        StartCoroutine(HideMenuAndCursor());
    }
    public void SettingsButton()
    {
        SettingsUi.SetActive(true);
        MenuUi.SetActive(false);
    }
    public void BackButton()
    {
        SettingsUi.SetActive(false);
        MenuUi.SetActive(true);
    }
public void MenuButton()
    {
        SceneLoader.Instance.LoadScene(0);
    }
}
