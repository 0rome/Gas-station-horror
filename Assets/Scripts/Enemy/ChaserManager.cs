using UnityEngine;
using UnityEngine.AI;

public class ChaserManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int callDistance = 80;
    [SerializeField] private GameObject chaserObject;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform gasStationTransform;

    [Header("Audio")]
    [SerializeField] private AudioSource roarSound;


    private LightSwitch[] lightSwitches;
    private bool isChasing;
    private bool lightsOff;

    private void Start()
    {
        lightSwitches = FindObjectsByType<LightSwitch>(FindObjectsSortMode.None);
    }

    public void CallChaser()
    {
        roarSound.Play();

        if (chaserObject.activeSelf == false)
        {
            isChasing = true;
            chaserObject.SetActive(true);
            chaserObject.GetComponent<ChaserEnemy>().SetTarget();
            chaserObject.transform.position = startPoint.position;
        }
        else
        {
            chaserObject.GetComponent<ChaserEnemy>().SetTarget();
        }
    }
    public void RecallChaser()
    {
        isChasing = false;
        lightsOff = false;
        chaserObject.GetComponent<ChaserEnemy>().SetTarget(endPoint);
    }

    private void Update()
    {
        if (Vector3.Distance(endPoint.position,chaserObject.transform.position) < 0.5f && !isChasing)
        {
            chaserObject.SetActive(false);
        }
        if (Vector3.Distance(playerTransform.position,gasStationTransform.position) > callDistance && !isChasing)
        {
            CallSystem.instance.AddValue(100);
        }

        var agent = chaserObject.GetComponent<NavMeshAgent>();

        float distance = Vector3.Distance(playerTransform.position, gasStationTransform.position);

        if (!chaserObject.GetComponent<ChaserEnemy>().isGetingDamage)
        {
            agent.speed = Mathf.Clamp(distance / 20f, 4f, 10f);
        }
        if (isChasing && !lightsOff)
        {
            foreach (var item in lightSwitches)
            {
                if (item.IsOn)
                {
                    item.DoSomething();
                }
            }
            lightsOff = true;
        }
    }
}
