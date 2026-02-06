using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private Dictionary<IPlotAction, bool> plotActions = new Dictionary<IPlotAction, bool>();

    private IPlotAction transformatorGlitch;
    private IPlotAction phoneCall_1;
    private IPlotAction transformatorScreamer;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        // Находим объект, реализующий интерфейс
        transformatorGlitch = FindFirstObjectByType<LightTransformatorGlitch>();
        phoneCall_1 = FindFirstObjectByType<Phone>();
        transformatorScreamer = FindFirstObjectByType<TransformatorScreamer>();



        // Добавляем в словарь
        plotActions[transformatorGlitch] = false;
        plotActions[phoneCall_1] = false;
        plotActions[transformatorScreamer] = false;


        StartCoroutine(MainPlot());
    }

    private IEnumerator MainPlot()
    {
        yield return new WaitForSeconds(5f);

        SetAction(phoneCall_1,true);

        yield return new WaitUntil(() => phoneCall_1.isEnd());

        SetAction(transformatorGlitch, true);

        yield return new WaitUntil(() => transformatorGlitch.isEnd());

        SetAction(transformatorScreamer,true);

        yield return new WaitUntil(() => transformatorScreamer.isEnd());
    }

    public void SetAction(IPlotAction action, bool value)
    {
        if (action == null)
        {
            Debug.LogWarning("Попытка установить флаг для null-действия!");
            return;
        }

        if (plotActions.ContainsKey(action))
        {
            plotActions[action] = value;
            action.Action(); // выполняем действие
            Debug.Log("Запущен" + action);
        }
        else
        {
            Debug.LogWarning($"Флаг для {action} не найден!");
        }
    }

    public bool GetAction(IPlotAction action)
    {
        if (plotActions.TryGetValue(action, out bool value))
        {
            return value;
        }

        Debug.LogWarning($"Флаг для {action} не найден!");
        return false;
    }
}
