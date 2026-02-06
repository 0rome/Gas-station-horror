using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource[] audioSources;

    public void PlaySound(int soundIndex)
    {
        audioSources[soundIndex].Play();
    }
    public void StopSound(int soundIndex)
    {
        audioSources[soundIndex].Stop();
    }
}
