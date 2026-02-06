using System.Collections;
using UnityEngine;

public class LightTransformatorGlitch : BasePlotAction
{
    [SerializeField] private LightSwitch lightTransformator;
    [SerializeField] private Transform Point;
    private SoundPlayer soundPlayer;

    void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();
    }

    public override void Action() 
    {
        StartCoroutine(TransformatorGlitch());
        
    }
    private IEnumerator TransformatorGlitch()
    {
        lightTransformator.AllOff();

        soundPlayer.PlaySound(1);
        actionCompleted = true;

        yield return new WaitForSeconds(3);

        soundPlayer.PlaySound(2);
    }
}
