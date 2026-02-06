using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class AmbientZoneBlender : MonoBehaviour
{
    [SerializeField] private Collider AreaCollider;
    [SerializeField] GameObject playerObject;

    private void Update()
    {
        Vector3 closestPoint = AreaCollider.ClosestPoint(playerObject.transform.position);
        transform.position = closestPoint;
    }
}
