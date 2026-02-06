using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Activity controll")]
    public bool canMove = true;
    public bool canRotate = true;

    [Header("ControlData")]
    [SerializeField] private ControlData controlData;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform cameraPointTransform;
    [SerializeField] private float rotationSmoothTime = 0.05f; // регулирует плавность
    [SerializeField] private float horizontalSmoothMultiplier = 0.2f; // чуть меньше сглаживания по горизонтали
    [SerializeField] private float axisSmoothSpeed = 10f;

    [Header("Camera Bobbing")]
    [SerializeField] private bool enableCameraBobbing = true;
    [SerializeField] private float walkBobbingSpeed = 14f;
    [SerializeField] private float runBobbingSpeed = 18f;
    [SerializeField] private float walkBobbingAmount = 0.05f;
    [SerializeField] private float runBobbingAmount = 0.08f;
    [SerializeField] private float bobSmoothness = 2f;

    [Header("Camera Tilt")]
    [SerializeField] private bool enableCameraTilt = true;
    [SerializeField] private float tiltAmount = 3f; // угол наклона при движении
    [SerializeField] private float tiltSmoothness = 5f; // плавность наклона

    [Header("Footsteps")]
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float runStepInterval = 0.35f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] groundSteps;
    [SerializeField] private AudioClip[] asphaltSteps;
    [SerializeField] private AudioClip[] platesSteps;
    [SerializeField] private AudioClip[] carpetSteps;
    [SerializeField] private LayerMask groundMask;

    private CharacterController controller;
    private Animator animator;

    private float verticalVelocity;
    private float xRotation = 0f;
    private float forwardSpeed;
    private float rightSpeed;
    private float stepTimer;
    private bool cameraLocked = false;

    // Camera bobbing variables
    private float defaultCameraY;
    private float bobbingTimer = 0f;
    private Vector3 originalCameraPosition;
    private bool isRunning;

    private float smoothMouseX;
    private float smoothMouseY;
    private float smoothX = 0f;
    private float smoothZ = 0f;
    private float xVelocity;
    private float yVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        // Сохраняем оригинальную позицию камеры
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
            defaultCameraY = originalCameraPosition.y;
        }

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleMovement();

        if (!cameraLocked)
        {
            HandleCameraRotation();

            if (enableCameraBobbing)
                HandleCameraBobbing();
        }

        HandleFootsteps();
    }


    private void HandleMovement()
    {
        if (cameraLocked) return;

        if (!canMove)
        {
            // Обнуляем движение и скорость
            smoothX = 0f;
            smoothZ = 0f;
            forwardSpeed = 0f;
            rightSpeed = 0f;
            isRunning = false;

            // Если есть аниматор — ставим параметры в 0
            if (animator)
            {
                animator.SetFloat("forwardSpeed", 0f);
                animator.SetFloat("rightSpeed", 0f);
            }

            // Обнуляем шаги
            stepTimer = 0f;

            // Камера должна постепенно возвращаться
            if (cameraTransform != null)
            {
                cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, originalCameraPosition, bobSmoothness * Time.deltaTime);
                // Можно и наклон обнулить
                cameraTransform.localRotation = Quaternion.Slerp(cameraTransform.localRotation, Quaternion.Euler(xRotation, 0f, 0f), Time.deltaTime * tiltSmoothness);
            }

            return; // Прерываем выполнение движения
        }


        float targetX = 0f;
        float targetZ = 0f;

        // Определяем целевые значения
        if (Input.GetKey(controlData.moveUpKey)) targetZ = 1f;
        if (Input.GetKey(controlData.moveDownKey)) targetZ = -1f;

        if (Input.GetKey(controlData.moveRightKey)) targetX = 1f;
        if (Input.GetKey(controlData.moveLeftKey)) targetX = -1f;

        // 🔥 ПЛАВНО ПЕРЕХОДИМ К ЦЕЛИ (как Input.GetAxis)
        smoothX = Mathf.Lerp(smoothX, targetX, Time.deltaTime * axisSmoothSpeed);
        smoothZ = Mathf.Lerp(smoothZ, targetZ, Time.deltaTime * axisSmoothSpeed);

        Vector3 move = transform.right * smoothX + transform.forward * smoothZ;

        forwardSpeed = smoothZ;
        rightSpeed = smoothX;

        isRunning = Input.GetKey(controlData.runKey);

        float currentSpeed = isRunning ? speed * 1.5f : speed;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (animator)
        {
            animator.SetFloat("forwardSpeed", smoothZ * (isRunning ? 1.5f : 1f));
            animator.SetFloat("rightSpeed", smoothX * (isRunning ? 1.5f : 1f));
        }

        // Гравитация
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -2f;
        else
            verticalVelocity += gravity * Time.deltaTime;

        Vector3 gravityMove = Vector3.up * verticalVelocity * Time.deltaTime;
        controller.Move(gravityMove);
    }



    private void HandleCameraRotation()
    {
        if (!canRotate) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * controlData.mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * controlData.mouseSensitivity;

        // Плавное сглаживание движения мыши
        smoothMouseX = Mathf.SmoothDamp(smoothMouseX, mouseX, ref xVelocity, rotationSmoothTime * horizontalSmoothMultiplier);
        smoothMouseY = Mathf.SmoothDamp(smoothMouseY, mouseY, ref yVelocity, rotationSmoothTime);

        // Обработка вертикального поворота камеры (вверх/вниз)
        xRotation -= smoothMouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        // Горизонтальный поворот игрока (влево/вправо)
        transform.Rotate(Vector3.up * smoothMouseX);

        // --- Камера ---
        // Увеличиваем наклон при беге
        float dynamicTilt = isRunning ? tiltAmount * 1.6f : tiltAmount;
        float targetTilt = enableCameraTilt ? -rightSpeed * dynamicTilt : 0f;

        // Плавное вращение камеры
        Quaternion targetCamRotation = Quaternion.Euler(xRotation, 0f, targetTilt);

        cameraTransform.localRotation = Quaternion.Slerp(
            cameraTransform.localRotation,
            targetCamRotation,
            Time.deltaTime * tiltSmoothness
        );
    }

    private void HandleCameraBobbing()
    {
        if (!canRotate) return;

        if (cameraTransform == null) return;

        bool isMoving = controller.isGrounded && (Mathf.Abs(forwardSpeed) > 0.1f || Mathf.Abs(rightSpeed) > 0.1f);

        if (isMoving)
        {
            // Выбираем параметры в зависимости от бега
            float bobbingSpeed = isRunning ? runBobbingSpeed : walkBobbingSpeed;
            float bobbingAmount = isRunning ? runBobbingAmount : walkBobbingAmount;

            // Учитываем движение в разных направлениях
            float horizontalMultiplier = Mathf.Abs(rightSpeed) > 0.1f ? 0.7f : 1f;
            float speedMultiplier = Mathf.Max(Mathf.Abs(forwardSpeed), Mathf.Abs(rightSpeed));

            bobbingTimer += Time.deltaTime * bobbingSpeed * speedMultiplier;

            // Создаем покачивание с использованием синуса и косинуса для более естественного движения
            float bobX = Mathf.Cos(bobbingTimer * 0.5f) * bobbingAmount * horizontalMultiplier;
            float bobY = defaultCameraY + Mathf.Sin(bobbingTimer) * bobbingAmount;

            Vector3 targetPosition = originalCameraPosition + new Vector3(bobX, bobY - originalCameraPosition.y, 0);

            // Плавно интерполируем к целевой позиции
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                targetPosition,
                bobSmoothness * Time.deltaTime
            );

            if (enableCameraTilt)
            {
                // Чем больше скорость по оси X, тем сильнее наклон
                float targetTilt = -rightSpeed * tiltAmount;

                // Плавно интерполируем текущий наклон
                Quaternion targetRotation = Quaternion.Euler(xRotation, 0f, targetTilt);
                cameraTransform.localRotation = Quaternion.Slerp(
                    cameraTransform.localRotation,
                    targetRotation,
                    Time.deltaTime * tiltSmoothness
                );
            }
        }
        else
        {
            // Плавно возвращаем камеру в исходное положение
            bobbingTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                originalCameraPosition,
                bobSmoothness * Time.deltaTime
            );
        }
    }

    private void HandleFootsteps()
    {
        bool isMoving = controller.isGrounded && (Mathf.Abs(forwardSpeed) > 0.1f || Mathf.Abs(rightSpeed) > 0.1f);
        if (!isMoving) { stepTimer = 0f; return; }

        stepTimer += Time.deltaTime;
        float interval = isRunning ? runStepInterval : walkStepInterval;

        if (stepTimer >= interval)
        {
            PlayFootstepSound();
            stepTimer = 0f;
        }
    }

    private void PlayFootstepSound()
    {
        if (!controller.isGrounded) return;

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, 2f, groundMask))
        {
            string surfaceTag = hit.collider.tag;
            AudioClip clip = null;

            switch (surfaceTag)
            {
                case "Ground":
                    if (groundSteps.Length > 0)
                        clip = groundSteps[Random.Range(0, groundSteps.Length)];
                    break;
                case "Asphalt":
                    if (asphaltSteps.Length > 0)
                        clip = asphaltSteps[Random.Range(0, asphaltSteps.Length)];
                    break;
                case "Plates":
                    if (platesSteps.Length > 0)
                        clip = platesSteps[Random.Range(0, platesSteps.Length)];
                    break;
                case "Carpet":
                    if (carpetSteps.Length > 0)
                        clip = carpetSteps[Random.Range(0, carpetSteps.Length)];
                    break;
            }

            if (clip != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.volume = isRunning ? 1f : 0.7f;
                audioSource.PlayOneShot(clip);
            }
        }
    }

    // Метод для сброса позиции камеры (можно вызывать при телепортации и т.д.)
    public void ResetCameraPosition()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localPosition = originalCameraPosition;
        }
    }
    public void DeactivePlayer()
    {
        canMove = false;
        canRotate = false;
    }
    public void ActivePlayer()
    {
        canMove = true;
        canRotate = true;   
    }
    public void LockCamera() { cameraLocked = true; }

    public void UnlockCamera()
    {
        cameraLocked = false; 
        Camera.main.transform.SetParent(transform);
        //Camera.main.transform.localPosition = cameraPointTransform.position;
        //Camera.main.transform.localRotation = Quaternion.identity;
    }
}