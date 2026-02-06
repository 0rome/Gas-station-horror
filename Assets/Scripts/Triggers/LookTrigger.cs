using UnityEngine;

public class LookTrigger : MonoBehaviour
{

    [Header("Target")]
    [SerializeField] private Transform targetObject; // За кем следим

    [Header("Settings")]
    [SerializeField] private float maxDistance = 1000f;
    [SerializeField] private float angleThreshold = 5f;

    private Camera playerCamera;

    private bool isLookingAtTarget = false;


    private void Start()
    {
        playerCamera = Camera.main;
    }

    private void Update()
    {
        if (playerCamera == null || targetObject == null)
            return;

        Vector3 camPos = playerCamera.transform.position;
        Vector3 camForward = playerCamera.transform.forward;

        // Направление к объекту
        Vector3 dirToTarget = (targetObject.position - camPos).normalized;

        // Проверка угла
        float angle = Vector3.Angle(camForward, dirToTarget);

        if (angle < angleThreshold)
        {
            // Проверяем, не закрыт ли объект
            if (Physics.Raycast(camPos, dirToTarget, out RaycastHit hit, maxDistance))
            {
                if (hit.transform == targetObject)
                {
                    if (!isLookingAtTarget)
                    {
                        isLookingAtTarget = true;
                        OnStartLooking();
                    }
                    return;
                }
            }
        }

        // Если перестали смотреть
        if (isLookingAtTarget)
        {
            isLookingAtTarget = false;
            OnStopLooking();
        }
    }

    private void OnStartLooking()
    {
        Debug.Log("Игрок посмотрел на объект!");
        // Тут можешь вызвать событие или действие
    }

    private void OnStopLooking()
    {
        Debug.Log("Игрок перестал смотреть на объект!");
    }
}
