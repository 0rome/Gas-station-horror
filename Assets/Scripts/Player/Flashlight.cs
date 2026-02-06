using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private GameObject FlashlightObject;
    [SerializeField] private TwoBoneIKConstraint ikConstraint;
    [SerializeField] private float transitionSpeed = 3f;
    [SerializeField] private float turnOffDelay = 0.5f;

    private bool isFlashlightOn = false;
    private float targetWeight = 0f;
    private SoundPlayer soundPlayer;

    private void Start()
    {
        soundPlayer = FlashlightObject.GetComponent<SoundPlayer>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isFlashlightOn)
            {
                // Сначала включаем IK, потом с задержкой выключаем фонарик
                targetWeight = 0f;
                Invoke(nameof(TurnOffFlashlight), turnOffDelay);
                soundPlayer.PlaySound(0);
            }
            else
            {
                // Включаем фонарик и выключаем IK
                FlashlightObject.SetActive(true);
                targetWeight = 1f;
                isFlashlightOn = true;
                soundPlayer.PlaySound(0);
            }
        }

        // Плавное изменение веса
        if (ikConstraint != null)
        {
            ikConstraint.weight = Mathf.Lerp(
                ikConstraint.weight,
                targetWeight,
                transitionSpeed * Time.deltaTime
            );
        }
    }

    private void TurnOffFlashlight()
    {
        FlashlightObject.SetActive(false);
        isFlashlightOn = false;
    }
}