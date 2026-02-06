using UnityEngine;
using System.IO;

public class ScreenshotTool : MonoBehaviour
{
    [Header("Основные настройки")]
    [Tooltip("Префикс для имени файла")]
    public string fileNamePrefix = "Screenshot";

    [Tooltip("Множитель разрешения (1 = оригинальное)")]
    public int superSize = 1;

    [Header("Авто-скриншоты")]
    public bool takeAutoScreenshots = false;
    public float interval = 5f;

    private float timer = 0f;
    private int screenshotCount = 0;

    void Update()
    {
        // Ручной скриншот по нажатию F12
        if (Input.GetKeyDown(KeyCode.F12))
        {
            TakeScreenshot();
        }

        // Ручной скриншот по нажатию S (с Shift для супер-разрешения)
        if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftShift))
        {
            superSize = 4;
            TakeScreenshot();
            superSize = 1;
        }

        // Автоматические скриншоты
        if (takeAutoScreenshots)
        {
            timer += Time.deltaTime;
            if (timer >= interval)
            {
                TakeScreenshot();
                timer = 0f;
            }
        }
    }

    /// <summary>
    /// Сделать скриншот
    /// </summary>
    public void TakeScreenshot()
    {
        // Создаем папку если её нет
        string folderPath = Path.Combine(Application.dataPath, "Screenshots");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // Генерируем имя файла с временной меткой
        string timestamp = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = $"{fileNamePrefix}_{timestamp}_{screenshotCount}.png";
        string filePath = Path.Combine(folderPath, fileName);

        // Делаем скриншот
        ScreenCapture.CaptureScreenshot(filePath, superSize);

        Debug.Log($"Скриншот сохранен: {filePath}");
        screenshotCount++;
    }

    /// <summary>
    /// Сделать скриншот с заданным именем
    /// </summary>
    public void TakeScreenshot(string customName)
    {
        string folderPath = Path.Combine(Application.dataPath, "Screenshots");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, $"{customName}.png");
        ScreenCapture.CaptureScreenshot(filePath, superSize);

        Debug.Log($"Скриншот сохранен: {filePath}");
    }
}