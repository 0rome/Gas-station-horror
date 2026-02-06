using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;

    [Header("UI")]
    [SerializeField] private CanvasGroup loadingScreen; // Важно: CanvasGroup!
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Animation Settings")]
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float fakeFinishDelay = 0.25f;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // В начале экран невидим
        loadingScreen.alpha = 0f;
        loadingScreen.gameObject.SetActive(false);
    }

    // Загрузка по индексу
    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadAsync(sceneIndex));
    }

    // Загрузка по имени
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsync(sceneName));
    }

    private IEnumerator LoadAsync(int sceneIndex)
    {
        yield return StartCoroutine(LoadRoutine(SceneManager.LoadSceneAsync(sceneIndex)));
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        yield return StartCoroutine(LoadRoutine(SceneManager.LoadSceneAsync(sceneName)));
    }

    // Основная логика загрузки
    private IEnumerator LoadRoutine(AsyncOperation operation)
    {
        operation.allowSceneActivation = false;

        // Включаем экран, делаем fade-in
        loadingScreen.gameObject.SetActive(true);

        loadingScreen.alpha = 0f;
        loadingScreen.DOFade(1f, fadeDuration);

        loadingSlider.value = 0f;

        float smoothProgress = 0f;

        while (!operation.isDone)
        {
            float targetProgress = Mathf.Clamp01(operation.progress / 0.9f);

            // Плавно двигаем значение ползунка
            smoothProgress = Mathf.MoveTowards(smoothProgress, targetProgress, Time.deltaTime);

            loadingSlider.DOValue(smoothProgress, 0.15f);

            if (loadingText != null)
                loadingText.text = (smoothProgress * 100f).ToString("F0") + "%";

            if (smoothProgress >= 0.99f)
            {
                yield return new WaitForSeconds(fakeFinishDelay);

                // Завершение загрузки → разрешаем активацию сцены
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        // Fade-out и отключение экрана
        yield return loadingScreen.DOFade(0f, fadeDuration).WaitForCompletion();
        loadingScreen.gameObject.SetActive(false);
    }
}
