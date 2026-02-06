using UnityEngine;

public class Pistol : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float range = 100f;
    [SerializeField] private int maxBullets = 12;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private SoundPlayer soundPlayer;
    [SerializeField] private GameObject gilzaObject;
    [SerializeField] private Transform gilzaSpawnTransform;

    private int currentBullets;
    private float nextShootTime = 0f;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentBullets = maxBullets;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextShootTime)
        {
            if (currentBullets <= 0) { soundPlayer.PlaySound(1); return; }

            nextShootTime = Time.time + fireRate;
            Shoot();
        }
    }

    private void Shoot()
    {
        currentBullets--;

        // Вспышка
        if (muzzleFlash != null)
            muzzleFlash.Play();

        // Звук
        if (soundPlayer != null)
            soundPlayer.PlaySound(0);

        if (gilzaObject != null)
            Instantiate(gilzaObject, gilzaSpawnTransform.position, gilzaSpawnTransform.rotation);


        animator.SetTrigger("Shoot");

        // RayCast пули
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, range))
        {
            // Попадание
            if (hitEffect != null)
                Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));

            //Урон врагу
            var health = hit.collider.GetComponent<ChaserEnemy>();
            if (health != null)
                health.GetDamage();
        }
    }
}