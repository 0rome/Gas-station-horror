using System.Drawing;
using UnityEngine;

public class TransformatorScreamer : BasePlotAction
{
    [Header("Sounds")]
    [SerializeField] private AudioSource screamerSound;


    [Header("Screamer Settings")]
    [SerializeField] private Transform point;
    [SerializeField] private GameObject screamerObject;
    [SerializeField] private LightSwitch transformatorSwitch;

    [Header("Look Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private float lookAngle = 5f;
    [SerializeField] private float maxLookDistance = 15f;

    private PlayerController playerController;
    private bool isActivated = false;
    private bool isLooking = false;

    void Start()
    {
        playerController = FindFirstObjectByType<PlayerController>();

        // Если камера не указана — берём основную
        if (playerCamera == null)
            playerCamera = Camera.main;

        // если цель не указана — триггер смотрят на сам объект
        if (lookTarget == null)
            lookTarget = transform;
    }

    void Update()
    {
        CheckLook();

        if (point == null) return;
        if (isActivated && Vector3.Distance(point.position, playerCamera.transform.position) <= 0.5f)
        {
            screamerObject.SetActive(true);
            playerController.canMove = false;
        }
    }

    public override void Action()
    {
        isActivated = true;
    }

    /// <summary>
    /// Проверяет, смотрит ли игрок на объект
    /// </summary>
    private void CheckLook()
    {
        if (playerCamera == null || lookTarget == null) return;
        if (!lookTarget.gameObject.activeInHierarchy) return; // <--- добавили проверку

        Vector3 camPos = playerCamera.transform.position;
        Vector3 camForward = playerCamera.transform.forward;
        Vector3 dirToObject = (lookTarget.position - camPos).normalized;

        float angle = Vector3.Angle(camForward, dirToObject);
        float distance = Vector3.Distance(camPos, lookTarget.position);

        if (angle <= lookAngle && distance <= maxLookDistance)
        {
            if (!isLooking)
            {
                isLooking = true;
                Debug.Log("Игрок посмотрел на объект.");
                screamerSound.Play();
                Invoke(nameof(Deactivate), 0.3f);
            }
        }
        else
        {
            isLooking = false;
        }
    }
    private void Deactivate()
    {
        transformatorSwitch.DoSomething();
        playerController.canMove = true;
        screamerObject.SetActive(false);
        point = null;
        point.gameObject.SetActive(false);
    }
}
