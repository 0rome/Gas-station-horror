using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ChaserEnemy : MonoBehaviour
{
    public float chaseDistance = 10f;
    public float catchDistance = 1.5f;

    public bool canMove { get; set; } = true;
    public bool isGetingDamage { get; set; } = false;

    private bool canPlayMelody = true;

    private NavMeshAgent agent;
    private Transform player;
    private Animator animator;
    private Transform agentTarget;
    private SoundPlayer soundPlayer;

    void Start()
    {
        soundPlayer = GetComponentInChildren<SoundPlayer>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(RandomScream());
        agentTarget = player;
    }

    void Update()
    {
        if (!canMove) return;
        agent.SetDestination(agentTarget.position);
        animator.speed = agent.speed / 3f;
        if (Vector3.Distance(transform.position, player.transform.position) <= 20 && canPlayMelody)
        {
            soundPlayer.PlaySound(0);
            canPlayMelody = false;
            StartCoroutine(ReloadMelody());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Door door = other.GetComponent<Door>();
        if (door != null && !door.isOpened)
        {
            door.DoSomething();
        }
        if (other.tag == "Player")
        {
            FindFirstObjectByType<CutsceneManager>().PlayCutscene(1);
            agent.speed = 0;
            canMove = false;
            animator.speed = 1;
            Debug.Log("Player caught by ChaserEnemy!");
        }

    }
    public void PlayCatchAnimation()
    {
        animator.SetTrigger("Catch");
    }
    public void GetDamage()
    {
        StartCoroutine(getDamage());
    }
    private IEnumerator getDamage()
    {
        isGetingDamage = true;
        animator.SetTrigger("GetDamage");
        agent.speed = 0;
        animator.applyRootMotion = true;
        yield return new WaitForSeconds(2f);
        agent.speed = 3.5f;
        isGetingDamage = false;
        animator.applyRootMotion = false;
    }


    private IEnumerator RandomScream()
    {
        while (true)
        {
            yield return new WaitForSeconds(8);
            soundPlayer.PlaySound(Random.Range(1,4));
        }
    }

    private IEnumerator ReloadMelody()
    {
        yield return new WaitForSeconds(120);
        canPlayMelody = true;
    }

    public void SetTarget(Transform target) { agentTarget = target; }
    
    public void SetTarget() { agentTarget = player; }
    
} 
