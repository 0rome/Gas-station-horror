using UnityEngine;

public class CarTrigger : MonoBehaviour
{
    [SerializeField] private bool stopCar;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponent<Car>())
        {
            collision.GetComponent<Car>().canMove = !stopCar;
        }
    }
}
