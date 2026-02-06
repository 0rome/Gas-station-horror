using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnim : MonoBehaviour, IPointerEnterHandler
{
    private Button button;

    private AudioSource[] sources;

    void Start()
    {

        sources = GetComponents<AudioSource>();

        button = GetComponent<Button>();
        button.onClick.AddListener(() => button.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f));
        
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if(sources.Length > 0) sources[1].Play();

        
    }
}
