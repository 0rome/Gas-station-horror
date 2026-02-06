using System.Collections;
using UnityEngine;

public class Phone : BasePlotAction, IInteractable
{
    private SoundPlayer soundPlayer;
    private Collider Collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();
        Collider = GetComponent<Collider>();
        Collider.enabled = false;
    }

    public void DoSomething()
    {
        StartCoroutine(Voice());

        soundPlayer.StopSound(1);
    }


    public override void Action()
    {
        Collider.enabled = true;
        soundPlayer.PlaySound(1);
    }


    private IEnumerator Voice()
    {
        soundPlayer.PlaySound(2);
        Collider.enabled = false;

        yield return new WaitForSeconds(1f);

        soundPlayer.PlaySound(0);

        yield return new WaitForSeconds(21f);

        actionCompleted = true;
        Collider.enabled = false;
    }
}
