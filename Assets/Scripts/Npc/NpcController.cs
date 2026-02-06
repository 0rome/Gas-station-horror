using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NpcController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject[] productItems;
    [SerializeField] private GameObject[] moneyPrefabs;
    [SerializeField] private CashMonitor cashMonitor;

    [Header("Ui")]
    [SerializeField] private Image timerSlider;

    [Header("Points")]
    [SerializeField] private Transform handPoint;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform sittingPoint;
    [SerializeField] private Transform cashRegisterPoint;
    [SerializeField] private Transform moneyPoint;

    [Header("Our car")]
    [SerializeField] private Car car;

    [Header("Audio")]
    [SerializeField] private AudioSource stepAudio;

    [HideInInspector] public GameObject[] allNeedItems;
    private NavMeshAgent agent;
    private Transform agentTarget;
    private GameObject currentItem;
    private int currentItemIndex;
    private Tween sliderTween;

    public bool needPutMoney { get; set; } = false;
    private bool isTakingItem = false;
    private bool allItemsCollected = false;
    private bool isPuttingMoney = false;
    private bool inCar = true;
    private bool goingBack = false;
    private bool isGetChange = false;

    private Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Door>())
        {
            other.GetComponent<Door>().OpenDoor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (agent == null || !agent.isActiveAndEnabled) return;

        if (inCar) return; // Если в машине, ничего не делаем

        HandleFootsteps();

        if (!agent.isStopped && agentTarget != null)
        {
            agent.SetDestination(agentTarget.position);
        }

        GetInCar();

        if (!inCar && currentItem != null &&
            Vector3.Distance(agent.transform.position, currentItem.transform.position) <= 2 &&
            !isTakingItem)
        {
            isTakingItem = true;
            StartCoroutine(takeItem());
        }

        if (needPutMoney && !isPuttingMoney)
        {
            agentTarget = cashRegisterPoint;

            if (Vector3.Distance(transform.position, cashRegisterPoint.position) <= 1f)
            {
                isPuttingMoney = true;
                StartCoroutine(PutMoney());
            }
        }
    }

    private IEnumerator takeItem()
    {
        if (currentItem == null) yield break;

        LookAt(currentItem.transform);
        agent.isStopped = true;
        animator.SetTrigger("Take");

        yield return new WaitForSeconds(2f);

        if (currentItem != null)
        {
            currentItem.transform.SetParent(handPoint);
            currentItem.transform.localPosition = Vector3.zero;
        }

        yield return new WaitForSeconds(2.7f);

        isTakingItem = false;
        agent.isStopped = false;
        agent.speed = 3;

        if (!allItemsCollected && currentItemIndex + 1 < allNeedItems.Length)
        {
            currentItemIndex++;
            currentItem = allNeedItems[currentItemIndex];
            agentTarget = currentItem.transform;
            Debug.Log($"Перешел к следующему предмету {currentItemIndex + 1}/{allNeedItems.Length}");
        }
        else
        {
            Debug.Log("Все предметы собраны, иду платить");
            currentItem = null;
            allItemsCollected = true;
            needPutMoney = true;
            agentTarget = cashRegisterPoint;
        }
    }
    private IEnumerator PutMoney()
    {
        agent.isStopped = true;
        LookAt(moneyPoint.transform);
        animator.SetTrigger("Put");

        yield return new WaitForSeconds(2f);

        Instantiate(moneyPrefabs[Random.Range(0, moneyPrefabs.Length)], moneyPoint.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(1.3f);

        cashMonitor.SetMonitorData(this);
        StartCoroutine(WaitChange());

        yield return new WaitUntil(() => isGetChange);

        agentTarget = startPoint;
        goingBack = true;
        agent.isStopped = false;
        agent.speed = 3;

    }

    private IEnumerator WaitChange()
    {
        float timer = 0f;
        float duration = 60f;

        Vector3 originalScale = timerSlider.transform.localScale;
        sliderTween = timerSlider.transform
            .DOScale(originalScale * 1.1f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);

        while (timer < duration)
        {
            if (isGetChange)
            {
                timerSlider.fillAmount = 0f;
                sliderTween.Kill();
                yield break;
            }

            timerSlider.fillAmount = Mathf.Lerp(timerSlider.fillAmount, timer / duration, 0.5f);

            timer += Time.deltaTime; // увеличиваем таймер
            yield return null;       // ждём следующий кадр
        }

        // Таймер закончился и isGetChange всё ещё false
        if (!isGetChange)
        {
            GetChange();
            cashMonitor.ResetData();
            CallSystem.instance.AddValue(100);
            timerSlider.fillAmount = 0f;
        }
    }

    public void GetChange()
    {
        isGetChange = true;
        animator.SetTrigger("GetChange");
    }
    private void HandleFootsteps()
    {
        // NPC движется
        bool isMoving = agent.velocity.magnitude > 0.1f && !agent.isStopped;

        if (isMoving)
        {
            if (!stepAudio.isPlaying)
            {
                stepAudio.Play();
            }
        }
        else
        {
            if (stepAudio.isPlaying)
            {
                stepAudio.Stop();
            }
        }
    }
    public void GetOutCar()
    {
        agent.enabled = true;
        gameObject.transform.position = startPoint.position;
        transform.parent = null;
        inCar = false;
        animator.SetBool("inCar",inCar);


        int itemCount = Mathf.Min(Random.Range(1, 6), productItems.Length);
        allNeedItems = new GameObject[itemCount];

        // Список доступных предметов
        List<GameObject> availableItems = new List<GameObject>(productItems);

        for (int i = 0; i < itemCount; i++)
        {
            int randomIndex = Random.Range(0, availableItems.Count);
            allNeedItems[i] = availableItems[randomIndex];
            availableItems.RemoveAt(randomIndex);
        }

        currentItemIndex = 0;
        currentItem = allNeedItems[0];
        agentTarget = currentItem.transform;
    }
    private void GetInCar()
    {
        if (!goingBack) return;

        if (Vector3.Distance(agent.transform.position, startPoint.transform.position) <= 1)
        {
            agent.enabled = false;
            transform.SetParent(sittingPoint);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(0,90,0);
            inCar = true;
            animator.SetBool("inCar", inCar);
            car.npcInCar = inCar;
            car.CloseSound();
        }
    }
    public void ResetNpc()
    {
        if (currentItem != null && currentItem.transform.parent == handPoint)
        {
            currentItem.transform.SetParent(null);
        }
        foreach (var item in allNeedItems)
        {
            item.transform.SetParent(null);
        }
        allNeedItems = null;
        currentItem = null;
        allNeedItems = null;

        allItemsCollected = false;
        isGetChange = false;
        isTakingItem = false;
        needPutMoney = false;
        isPuttingMoney = false;
        goingBack = false;

        currentItemIndex = 0;
    }

    private void LookAt(Transform lookObject)
    {
        Vector3 dir = (lookObject.position - transform.position);
        dir.y = 0;
        if (dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

}
