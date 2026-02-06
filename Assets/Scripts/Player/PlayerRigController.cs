using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerRigController : MonoBehaviour
{
    [Header("Control Data")]
    [SerializeField] private ControlData controlData;

    [Header("Constraints")]
    [SerializeField] private TwoBoneIKConstraint rightHandIk;
    [SerializeField] private Transform rightHandTarget;

    [Header("Items")]
    [SerializeField] private GameObject pistol;
    [SerializeField] private GameObject flashlight;

    [Header("Positions")]
    [SerializeField] private Vector3 flashlightPos;
    [SerializeField] private Vector3 flashlightRot;
    [SerializeField] private Vector3 pistolPos;
    [SerializeField] private Vector3 pistolRot;

    [Header("Settings")]
    [SerializeField] private float transitionSpeed = 3f;
    [SerializeField] private SoundPlayer itemsSound;
    [SerializeField] private bool deactivatePistol;
    [SerializeField] private bool deactivateFlashlight;

    private PlayerController playerController;
    private GameObject currentItem = null;
    private GameObject targetItem = null;
    private float targetWeight = 0f;


    private void Start()
    {
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (!playerController.canMove) return;

        if (Input.GetKeyDown(controlData.flashlightKey) && !deactivateFlashlight)
        {
            ToggleItem(flashlight);
            itemsSound.PlaySound(0);
        }
            

        if (Input.GetKeyDown(KeyCode.Alpha1) && !deactivatePistol)
        {
            ToggleItem(pistol);
            itemsSound.PlaySound(1);
        }
            

        UpdateIK();
    }

    // -----------------------
    //     MAIN LOGIC
    // -----------------------

    private void ToggleItem(GameObject item)
    {

        if (currentItem == item)
        {
            // Выключаем тот же самый предмет
            targetItem = null;
            targetWeight = 0f;
        }
        else
        {
            // Меняем предмет
            targetItem = item;
            targetWeight = 0f; // Сначала плавно опускаем руку
        }
    }

    private void UpdateIK()
    {
        if (rightHandIk == null) return;

        // Плавно меняем вес IK
        rightHandIk.weight = Mathf.Lerp(rightHandIk.weight, targetWeight, transitionSpeed * Time.deltaTime);

        // Когда IK почти выключен — меняем предмет
        if (rightHandIk.weight <= 0.05f && targetWeight == 0f)
        {
            ApplyItemSwitch();
        }

        // Если предмет активен — тянем руку к позиции
        if (currentItem != null)
        {
            Vector3 pos = currentItem == flashlight ? flashlightPos : pistolPos;
            Vector3 rot = currentItem == flashlight ? flashlightRot : pistolRot;

            rightHandTarget.localPosition = Vector3.Lerp(
                rightHandTarget.localPosition,
                pos,
                transitionSpeed * Time.deltaTime);

            rightHandTarget.localRotation = Quaternion.Lerp(
                rightHandTarget.localRotation,
                Quaternion.Euler(rot),
                transitionSpeed * Time.deltaTime);
        }

    }

    // Меняем предмет, когда рука опущена
    private void ApplyItemSwitch()
    {
        if (currentItem != null)
            currentItem.SetActive(false);

        currentItem = targetItem;

        if (currentItem != null)
        {
            currentItem.SetActive(true);
            targetWeight = 1f; // Поднимаем руку
        }
    }
}
