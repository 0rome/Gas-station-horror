using Unity.AppUI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Car : MonoBehaviour
{
    [Header("Car settings")]
    [SerializeField] private float speed = 5;
    [SerializeField] float gravityMultiplier = 4f;
    [SerializeField] private Animator animator;
    private float maxSpeed;

    [Header("Npc settings")]
    [SerializeField] private NpcController npcController;
    public bool isWent { get; set; } = false;
    public bool npcInCar { get; set; } = true;

    [Header("Audio settings")]
    [SerializeField] private AudioSource[] engineSounds;
    [SerializeField] private SoundPlayer soundPlayer;

    [Header("Wheels settings")]
    [SerializeField] private WheelCollider[] wheelsColliders;
    [SerializeField] private GameObject[] wheelsObjects;

    [Header("Path settings")]
    [SerializeField] public Transform[] pathPoints; // точки маршрута
    [SerializeField] private float reachThreshold = 0.5f; // на каком расстоянии считать точку достигнутой
    [SerializeField] private bool loop = false; // зациклить маршрут

    private NpcController[] allNpc;
    private Rigidbody rb;

    public bool canMove { get; set; } = true;

    private int currentPointIndex = 0;

    void Awake()
    {
        maxSpeed = speed;
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        allNpc = FindObjectsByType<NpcController>(FindObjectsSortMode.None);
    }
    void FixedUpdate()
    {
        Sounds();
        Gravity();
        MoveAlongPath();
        UpdateWheels();
        CheckNpc();

        if (!canMove || pathPoints.Length == 0)
        {
            speed = Mathf.Lerp(speed, 0f, Time.fixedDeltaTime / 3f);
            if (speed < 0.1f) { speed = 0f; }
            return;
        }
    }

    private void Sounds()
    {
        foreach (var sound in engineSounds)
        {
            sound.pitch = Mathf.Max(0.7f, Mathf.Lerp(sound.pitch, speed * 0.12f, Time.fixedDeltaTime / 3));
        }
    }

    private void MoveAlongPath()
    {
        Transform targetPoint = pathPoints[currentPointIndex];

        // 1. Направление к точке по плоскости XZ
        Vector3 targetPositionXZ = new Vector3(targetPoint.position.x, transform.position.y, targetPoint.position.z);
        Vector3 direction = (targetPositionXZ - transform.position).normalized;

        // 2. Поворот только по Y
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            targetRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // только ось Y
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 2f));
        }

        // 3. Движение вперёд
        rb.MovePosition(rb.position + transform.forward * speed * Time.fixedDeltaTime);

        // 4. Проверка достижения точки
        float distanceXZ = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z),
                                            new Vector3(targetPoint.position.x, 0, targetPoint.position.z));
        if (distanceXZ <= reachThreshold)
        {
            currentPointIndex++;
            if (currentPointIndex >= pathPoints.Length)
            {
                if (loop)
                    currentPointIndex = 0;
                else
                    canMove = false; // маршрут завершён
            }
        }
    }
    public void ResetPath()
    {
        npcController.ResetNpc();
        currentPointIndex = 0;
        canMove = true;
        isWent = false;
        npcInCar = true;
    }
    private void UpdateWheels()
    {
        for (int i = 0; i < wheelsColliders.Length; i++)
        {
            Vector3 pos;
            Quaternion rot;
            wheelsColliders[i].GetWorldPose(out pos, out rot);

            wheelsObjects[i].transform.position = pos;
            wheelsObjects[i].transform.Rotate(0,0,speed * Time.fixedDeltaTime * 100);
        }
    }

    private void Gravity()
    {
        rb.AddForce(Vector3.down * Physics.gravity.magnitude * (gravityMultiplier - 1),
               ForceMode.Acceleration);
    }

    private void CheckNpc()
    {
        if (speed == 0 && !isWent)
        {
            Debug.Log("Npc Get Out Car");
            isWent = true;
            npcInCar = false;
            npcController.GetOutCar();
            soundPlayer.PlaySound(0);
        }
        else if (isWent && npcInCar)
        {
            canMove = true;
            speed = Mathf.Lerp(speed, maxSpeed, Time.fixedDeltaTime / 3f);
        }
    }
    public void CloseSound()
    {
        soundPlayer.PlaySound(1);
    }
}
