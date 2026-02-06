using UnityEngine;
using System.Collections.Generic;

public class HorrorAudioOcclusionFixed : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private Transform listener;
    [SerializeField] private LayerMask occlusionLayers = ~0;

    [Header("Audio Sources (заполнится автоматически)")]
    [SerializeField] private List<AudioSource> audioSources = new List<AudioSource>();

    [Header("Настройки occlusion")]
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private float fadeSpeed = 5f;
    [SerializeField] private float wallVolumeMultiplier = 0.3f;
    [SerializeField] private float doorVolumeMultiplier = 0.6f;

    [Header("Настройки расстояния")]
    [SerializeField] private float maxDistance = 30f;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private AnimationCurve distanceCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("LowPass Filter")]
    [SerializeField] private bool useLowPassFilter = true;
    [SerializeField] private float minLowPassFrequency = 500f;
    [SerializeField] private float maxLowPassFrequency = 22000f;

    [Header("Автоматическая настройка")]
    [SerializeField] private bool autoConfigureAudioSources = false;
    [SerializeField] private bool includeChildren = true;

    // Приватные переменные
    private List<float> baseVolumes = new List<float>();
    private List<AudioLowPassFilter> lowPassFilters = new List<AudioLowPassFilter>();
    private List<bool> wasPlaying = new List<bool>(); // Запоминаем состояние воспроизведения
    private float currentVolumeMultiplier = 1f;
    private float currentLowPassMultiplier = 1f;
    private float timer = 0f;
    private float distanceToListener = 0f;

    void Awake()
    {
        // Ищем слушателя
        FindListener();

        // Находим все AudioSource
        FindAudioSources();

        // Сохраняем начальные состояния
        SaveInitialStates();

        // Настраиваем только если нужно
        if (autoConfigureAudioSources)
        {
            ConfigureAudioSources();
        }

        // Создаем LowPass фильтры если нужно
        if (useLowPassFilter)
        {
            CreateLowPassFilters();
        }
    }

    void FindListener()
    {
        if (listener == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                listener = player.transform;
                Debug.Log($"[AudioOcclusion] Найден слушатель: {listener.name}");
            }
            else if (Camera.main != null)
            {
                listener = Camera.main.transform;
                Debug.Log($"[AudioOcclusion] Используем камеру как слушателя");
            }
            else
            {
                Debug.LogWarning($"[AudioOcclusion] Не найден слушатель на {gameObject.name}");
            }
        }
    }

    void FindAudioSources()
    {
        // Очищаем списки
        audioSources.Clear();
        baseVolumes.Clear();
        lowPassFilters.Clear();
        wasPlaying.Clear();

        // Добавляем источники с этого объекта
        AudioSource[] selfSources = GetComponents<AudioSource>();
        foreach (var source in selfSources)
        {
            if (source != null && !audioSources.Contains(source))
            {
                audioSources.Add(source);
            }
        }

        // Добавляем дочерние если нужно
        if (includeChildren)
        {
            AudioSource[] childSources = GetComponentsInChildren<AudioSource>(true);
            foreach (var source in childSources)
            {
                if (source != null && !audioSources.Contains(source))
                {
                    audioSources.Add(source);
                }
            }
        }

       
    }

    void SaveInitialStates()
    {
        foreach (var source in audioSources)
        {
            if (source != null)
            {
                baseVolumes.Add(source.volume);
                wasPlaying.Add(source.isPlaying); // Запоминаем, играл ли звук изначально
            }
        }
    }

    void ConfigureAudioSources()
    {
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i] != null && audioSources[i].spatialBlend > 0.5f)
            {
                // Только для 3D звуков
                audioSources[i].spatialBlend = 1f;
                audioSources[i].minDistance = minDistance;
                audioSources[i].maxDistance = maxDistance;
                audioSources[i].dopplerLevel = 0.1f;

                // НЕ меняем Play On Awake!
                // НЕ меняем Loop!
                // НЕ запускаем звуки!
            }
        }
    }

    void CreateLowPassFilters()
    {
        foreach (var source in audioSources)
        {
            if (source != null && source.spatialBlend > 0.5f)
            {
                AudioLowPassFilter filter = source.GetComponent<AudioLowPassFilter>();
                if (filter == null)
                {
                    filter = source.gameObject.AddComponent<AudioLowPassFilter>();
                }
                filter.cutoffFrequency = maxLowPassFrequency;
                lowPassFilters.Add(filter);
            }
        }
    }

    void Update()
    {
        if (listener == null || audioSources.Count == 0)
        {
            // Если нет слушателя, используем дефолтные настройки
            if (listener == null)
                return;

            // Если нет аудиоисточников, выходим
            return;
        }

        timer += Time.deltaTime;
        if (timer >= checkInterval)
        {
            UpdateOcclusion();
            timer = 0f;
        }

        ApplyEffects();
    }

    void UpdateOcclusion()
    {
        distanceToListener = Vector3.Distance(transform.position, listener.position);

        // Если за пределами максимальной дистанции
        if (distanceToListener > maxDistance)
        {
            currentVolumeMultiplier = 0f;
            currentLowPassMultiplier = 0f;
            return;
        }

        // Расчет по расстоянию
        float distanceFactor = 1f - Mathf.Clamp01(
            (distanceToListener - minDistance) / (maxDistance - minDistance)
        );

        float volumeMultiplier = distanceCurve.Evaluate(1 - distanceFactor);
        float lowPassMultiplier = 1f;

        // Проверка стен (только если не слишком далеко)
        if (distanceToListener < maxDistance * 1.5f)
        {
            RaycastHit hit;
            Vector3 direction = (listener.position - transform.position).normalized;
            float rayDistance = Mathf.Min(distanceToListener, maxDistance);

            if (Physics.Raycast(transform.position, direction, out hit, rayDistance, occlusionLayers))
            {
                if (hit.collider.CompareTag("Wall"))
                {
                    volumeMultiplier *= wallVolumeMultiplier;
                    lowPassMultiplier *= 0.4f;
                }
                else if (hit.collider.CompareTag("Door"))
                {
                    volumeMultiplier *= doorVolumeMultiplier;
                    lowPassMultiplier *= 0.7f;
                }
                else if (hit.collider.CompareTag("Glass"))
                {
                    volumeMultiplier *= 0.8f;
                    lowPassMultiplier *= 0.9f;
                }
            }
        }

        currentVolumeMultiplier = Mathf.Clamp01(volumeMultiplier);
        currentLowPassMultiplier = Mathf.Clamp01(lowPassMultiplier);
    }

    void ApplyEffects()
    {
        // Применяем эффекты ко всем источникам
        for (int i = 0; i < audioSources.Count; i++)
        {
            if (audioSources[i] == null) continue;

            // Только для 3D звуков
            if (audioSources[i].spatialBlend > 0.5f)
            {
                // Вычисляем целевую громкость
                float targetVolume = baseVolumes[i] * currentVolumeMultiplier;

                // Плавно изменяем громкость
                audioSources[i].volume = Mathf.Lerp(
                    audioSources[i].volume,
                    targetVolume,
                    Time.deltaTime * fadeSpeed
                );
            }
        }

        // LowPass фильтры
        if (useLowPassFilter)
        {
            foreach (var filter in lowPassFilters)
            {
                if (filter != null)
                {
                    float targetFrequency = Mathf.Lerp(
                        minLowPassFrequency,
                        maxLowPassFrequency,
                        currentLowPassMultiplier
                    );

                    filter.cutoffFrequency = Mathf.Lerp(
                        filter.cutoffFrequency,
                        targetFrequency,
                        Time.deltaTime * fadeSpeed
                    );
                }
            }
        }
    }

    void OnValidate()
    {
        // В редакторе показываем предупреждение
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            AudioSource[] allSources = GetComponentsInChildren<AudioSource>(true);
            foreach (var source in allSources)
            {
                if (source.playOnAwake)
                {
                    
                }
            }
        }
#endif
    }

    void OnDrawGizmosSelected()
    {
        if (!enabled) return;

        // Рисуем дистанции
        Gizmos.color = new Color(0, 1, 0, 0.1f);
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = new Color(1, 0, 0, 0.05f);
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        // Луч к слушателю
        if (listener != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, listener.position);
        }
    }

    // === Публичные методы ===

    /// <summary>
    /// Добавить AudioSource вручную
    /// </summary>
    public void AddAudioSource(AudioSource source)
    {
        if (source != null && !audioSources.Contains(source))
        {
            audioSources.Add(source);
            baseVolumes.Add(source.volume);
            wasPlaying.Add(source.isPlaying);

            if (useLowPassFilter && source.spatialBlend > 0.5f)
            {
                AudioLowPassFilter filter = source.GetComponent<AudioLowPassFilter>();
                if (filter == null)
                    filter = source.gameObject.AddComponent<AudioLowPassFilter>();

                filter.cutoffFrequency = maxLowPassFrequency;
                lowPassFilters.Add(filter);
            }
        }
    }

    /// <summary>
    /// Удалить AudioSource
    /// </summary>
    public void RemoveAudioSource(AudioSource source)
    {
        int index = audioSources.IndexOf(source);
        if (index >= 0)
        {
            audioSources.RemoveAt(index);
            baseVolumes.RemoveAt(index);
            wasPlaying.RemoveAt(index);

            if (index < lowPassFilters.Count)
            {
                lowPassFilters.RemoveAt(index);
            }
        }
    }

    /// <summary>
    /// Обновить список AudioSource
    /// </summary>
    public void RefreshAudioSources()
    {
        FindAudioSources();
        SaveInitialStates();
    }

    /// <summary>
    /// Включить/выключить эффект occlusion
    /// </summary>
    public void SetOcclusionEnabled(bool enabled)
    {
        this.enabled = enabled;

        if (!enabled)
        {
            // Возвращаем исходную громкость
            for (int i = 0; i < audioSources.Count; i++)
            {
                if (audioSources[i] != null)
                {
                    audioSources[i].volume = baseVolumes[i];
                }
            }
        }
    }

    /// <summary>
    /// Получить текущий множитель громкости
    /// </summary>
    public float GetCurrentVolumeMultiplier()
    {
        return currentVolumeMultiplier;
    }

    /// <summary>
    /// Установить множитель стен вручную
    /// </summary>
    public void SetWallMultiplier(float multiplier)
    {
        wallVolumeMultiplier = Mathf.Clamp01(multiplier);
    }
}