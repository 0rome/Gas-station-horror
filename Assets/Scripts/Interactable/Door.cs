using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private float openDuration = 0.5f;
    [SerializeField] private Vector3 openAngles = new Vector3(0, -90, 0);

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] openClips;
    [SerializeField] private AudioClip[] closeClips;

    [HideInInspector] public bool isOpened;

    private Vector3 defaultRotation;
    private Coroutine addValueCoroutine;

    private void Start()
    {
        defaultRotation = transform.eulerAngles;
    }

    public void DoSomething()
    {
        if (!isOpened)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        if (isOpened) return;

        if (audioSource != null && openClips != null && openClips.Length > 0)
        {
            audioSource.pitch = Random.Range(1.2f, 1.5f);
            audioSource.PlayOneShot(openClips[Random.Range(0, openClips.Length)]);
        }

        Vector3 targetRotation = CalculateOpenRotation();

        transform.DORotate(targetRotation, openDuration);

        isOpened = true;

        // StartAddValueCoroutine();
    }

    public void CloseDoor()
    {
        if (!isOpened) return;

        // Проигрываем звук закрытия
        if (audioSource != null && closeClips != null && closeClips.Length > 0)
        {
            audioSource.pitch = Random.Range(1.2f, 1.5f);
            audioSource.PlayOneShot(closeClips[Random.Range(0, closeClips.Length)]);
        }

        transform.DORotate(NormalizeAngle(defaultRotation), openDuration);

        isOpened = false;

        StopAddValueCoroutine();
    }

    
    private Vector3 CalculateOpenRotation()
    {
        Vector3 targetRotation = defaultRotation;

        targetRotation.x = Mathf.Approximately(openAngles.x, 0f) ? defaultRotation.x : defaultRotation.x + openAngles.x;
        targetRotation.y = Mathf.Approximately(openAngles.y, 0f) ? defaultRotation.y : defaultRotation.y + openAngles.y;
        targetRotation.z = Mathf.Approximately(openAngles.z, 0f) ? defaultRotation.z : defaultRotation.z + openAngles.z;

        return NormalizeAngle(targetRotation);
    }

    
    private void StartAddValueCoroutine()
    {
        StopAddValueCoroutine();
        addValueCoroutine = StartCoroutine(AddCallValue());
    }

    
    private void StopAddValueCoroutine()
    {
        if (addValueCoroutine != null)
        {
            StopCoroutine(addValueCoroutine);
            addValueCoroutine = null;
        }
    }

    private IEnumerator AddCallValue()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (CallSystem.instance != null)
            {
                CallSystem.instance.AddValue(1);
            }
            else
            {
                break;
            }
        }
    }

    private Vector3 NormalizeAngle(Vector3 angles)
    {
        angles.x = (angles.x % 360 + 360) % 360;
        angles.y = (angles.y % 360 + 360) % 360;
        angles.z = (angles.z % 360 + 360) % 360;
        return angles;
    }

    private void OnDisable()
    {
        StopAddValueCoroutine();
    }
}