using System.Collections;
using UnityEngine;

public class SmileyGlass : BasePlotAction
{
    [SerializeField] private GameObject playerObj;
    [SerializeField] private GameObject triggerObj;
    [SerializeField] private SoundPlayer soundPlayer;

    private GameObject allObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundPlayer.PlaySound(0);

        allObjects = transform.GetChild(0).gameObject;
        allObjects.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(playerObj.transform.position, triggerObj.transform.position) <= 1f)
        {
            allObjects.SetActive(false);
        }
    }

    public override void Action()
    {
        allObjects.SetActive(true);
    }

}
