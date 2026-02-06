using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CarsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] carsObjects;
    [SerializeField] private Transform[] startPoints;
    [SerializeField] private Transform[] endPoints;

    [SerializeField] private Transform[] pathPoints_1;
    [SerializeField] private Transform[] pathPoints_2;

    private Car currentActiveCar;
    private int currentSpawnPoint;

    private void Start()
    {
        StartCoroutine(spawnCycle());
    }

    private void Update()
    {
        if (currentActiveCar != null && Vector3.Distance(currentActiveCar.transform.position, endPoints[currentSpawnPoint].position) <= 10)
        {
            currentActiveCar.ResetPath();
            currentActiveCar.gameObject.SetActive(false);
            currentActiveCar = null;
        }
    }

    public void SpawnCar()
    {
        currentActiveCar = carsObjects[Random.Range(0, carsObjects.Length)].GetComponent<Car>();
        currentActiveCar.gameObject.SetActive(true);

        currentSpawnPoint = Random.Range(0, startPoints.Length);

        currentActiveCar.transform.position = startPoints[currentSpawnPoint].position;
        currentActiveCar.transform.rotation = startPoints[currentSpawnPoint].rotation;

        if (currentSpawnPoint == 0) { currentActiveCar.pathPoints = pathPoints_1; }
        else { currentActiveCar.pathPoints = pathPoints_2; }
    }

    private IEnumerator spawnCycle()
    {
        while (true)
        {
            if (currentActiveCar == null)
            {
                SpawnCar();
            }

            yield return new WaitForSeconds(10f);
        }
    }
}
