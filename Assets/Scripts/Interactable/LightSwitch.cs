using UnityEngine;
using DG.Tweening;

public class LightSwitch : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    [SerializeField] private bool isTransformator;
    [SerializeField] private bool isOn = true;

    [Header("Objects")]
    [SerializeField] private GameObject switchObject;
    [SerializeField] private GameObject[] lightObjects;
    [SerializeField] private GameObject[] lampObjects;

    [Header("Materials")]
    [SerializeField] private Material emissionMaterial;
    [SerializeField] private Material defaultMaterial;

    public bool isWork { get; set; } = true;
    public bool IsOn => isOn;

    private SoundPlayer soundPlayer;


    private void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();
    }

    public void DoSomething()
    {
        soundPlayer?.PlaySound(0);
        if (!isWork) return;
        ToggleSwitch(!isOn);
    }

    public void AllOff()
    {
        if (!isWork) return;
        ToggleSwitch(false);
    }

    private void ToggleSwitch(bool value)
    {
        isOn = value;

        AnimateSwitch(value);
        UpdateLights(value);
        UpdateLampMaterials(value);
    }

    private void AnimateSwitch(bool value)
    {
        if (switchObject == null) return;

        if (!isTransformator)
        {
            float angle = value ? -40f : 40f;
            switchObject.transform.DOLocalRotate(new Vector3(angle, 0, 0), 0.5f);
        }
        else
        {
            float y = value ? -0.04f : 0f;
            switchObject.transform.DOLocalMoveY(y, 0.5f);
        }
    }

    private void UpdateLights(bool value)
    {
        if (lightObjects == null) return;

        foreach (var item in lightObjects)
        {
            if (item != null)
                item.SetActive(value);
        }
    }

    private void UpdateLampMaterials(bool value)
    {
        if (lampObjects == null) return;

        foreach (var item in lampObjects)
        {
            if (item == null) continue;

            var renderer = item.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = value ? emissionMaterial : defaultMaterial;
            }
        }
    }
}
