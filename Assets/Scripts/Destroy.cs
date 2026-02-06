using UnityEngine;

public class Destroy : MonoBehaviour
{
    [SerializeField] private float timeToDestroy = 1f;

    private void Start()
    {
        Destroy(gameObject, timeToDestroy);
    }
}
