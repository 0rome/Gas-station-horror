using System.Collections;
using UnityEngine;

public class Giant : MonoBehaviour
{
    [Header("Move settings")]
    [SerializeField] private float speed = 5.0f;

    [Header("Audio settings")]
    [SerializeField] private AudioSource headAudioSource;
    [SerializeField] private AudioSource stepAudioSource;
    [SerializeField] private Transform rightLegTransform;
    [SerializeField] private Transform leftLegTransform;


    void Start()
    {
        StartCoroutine(PlayRoarSound());
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void MakeStepSound(int legIndex)
    {
        stepAudioSource.pitch = Random.Range(1, 0.7f);
        if (legIndex == 0)
        {
            stepAudioSource.transform.position = rightLegTransform.position;
        }
        else
        {
            stepAudioSource.transform.position = leftLegTransform.position;
        }
        stepAudioSource.Play();
    }
    private IEnumerator PlayRoarSound()
    {
        while (true)
        {
            headAudioSource.pitch = Random.Range(0.5f, 0.7f);
            headAudioSource.Play();
            yield return new WaitForSeconds(20);
        }
    }
}
