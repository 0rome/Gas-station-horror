using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerInteract : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer = -1;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject interactImage;

    [Header("IK Settings")]
    [SerializeField] private TwoBoneIKConstraint ikConstraint;
    [SerializeField] private Transform handTarget; // 🎯 Target для руки
    [SerializeField] private float moveSpeed = 5f; // скорость движения руки

    [Header("Visualization")]
    [SerializeField] private bool showDebugRay = true;
    [SerializeField] private Color rayColor = Color.green;

    private Camera playerCamera;
    private IInteractable currentInteractable;
    private Transform lastHitTransform; // 👈 сюда сохраним объект, в который попали
    private bool isInteracting;
    private float targetWeight;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        CheckForInteractable();

        // Плавный переход веса
        if (ikConstraint != null)
        {
            ikConstraint.weight = Mathf.Lerp(ikConstraint.weight, targetWeight, 5 * Time.deltaTime);
        }

        // Когда нажали на взаимодействие
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && !isInteracting)
        {
            if (lastHitTransform != null)
                StartCoroutine(HandInteract(lastHitTransform));

            currentInteractable.DoSomething();
            currentInteractable = null;
            interactImage.SetActive(false);
        }
    }

    void CheckForInteractable()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance, interactableLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentInteractable = interactable;
                lastHitTransform = hit.transform; // 👈 сохраняем объект, в который попали
                interactImage.SetActive(true);
            }
            else
            {
                currentInteractable = null;
                lastHitTransform = null;
                interactImage.SetActive(false);
            }
        }
        else
        {
            currentInteractable = null;
            lastHitTransform = null;
            interactImage.SetActive(false);
        }

        if (showDebugRay)
            Debug.DrawRay(ray.origin, ray.direction * interactDistance, rayColor);
    }

    private IEnumerator HandInteract(Transform targetObject)
    {
        isInteracting = true;
        targetWeight = 1f;

        // Пока рука тянется к объекту
        float t = 0f;
        Vector3 startPos = handTarget.position;
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            handTarget.position = Vector3.Lerp(startPos, targetObject.position, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        // Возвращаем руку назад
        targetWeight = 0f;
        yield return new WaitForSeconds(0.3f);

        isInteracting = false;
    }
}
