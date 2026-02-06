using UnityEngine;

public class CameraFollowCursor : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float sensitivity = 0.05f; // Насколько камера реагирует на движение мыши
    [SerializeField] private float smoothSpeed = 5f;    // Плавность движения

    private Vector3 initialRotation;

    void Start()
    {
        // Запоминаем исходный угол камеры
        initialRotation = transform.eulerAngles;
    }

    void Update()
    {
        // Получаем позицию курсора на экране в диапазоне -1..1
        float mouseX = (Input.mousePosition.x / Screen.width - 0.5f) * 2f;
        float mouseY = (Input.mousePosition.y / Screen.height - 0.5f) * 2f;

        // Цель поворота камеры
        Vector3 targetRotation = new Vector3(
            initialRotation.x - mouseY * sensitivity * 100f,
            initialRotation.y + mouseX * sensitivity * 100f,
            initialRotation.z
        );

        // Плавное вращение к цели
        transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
