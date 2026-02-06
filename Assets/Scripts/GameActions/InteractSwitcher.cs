using System.Collections;
using UnityEngine;

public class InteractSwitcher : BasePlotAction
{
    [SerializeField] private GameObject[] interactableObjects;

    void Start()
    {
        StartCoroutine(RandomSwitch());
    }

    public override void Action()
    {
        RandomSwitch();
    }

    private IEnumerator RandomSwitch()
    {
        while (true)
        {
            yield return new WaitForSeconds(120f);
            interactableObjects[Random.Range(0, interactableObjects.Length)].GetComponent<IInteractable>().DoSomething();
        }
    }
}
