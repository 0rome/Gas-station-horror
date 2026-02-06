using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CallSystem : MonoBehaviour
{
    public static CallSystem instance { get; private set; }

    [SerializeField] private Image sliderImage;

    private ChaserManager chaserManager;
    private Color fillColor;
    private int callValue = 0;
    private bool isCalled = false;
    private Tween sliderTween;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        sliderImage.fillAmount = Mathf.Clamp01(callValue / 100f);
        chaserManager = FindFirstObjectByType<ChaserManager>();
        fillColor = sliderImage.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (callValue >= 100 && !isCalled)
        {
            isCalled = true;
            StartCoroutine(CallMax());
        }
        else if (callValue < 100 && isCalled)
        {
            isCalled = false;
        }
    }

    private IEnumerator CallMax()
    {
        Vector3 originalScale = sliderImage.transform.localScale;

        sliderTween = sliderImage.transform
            .DOScale(originalScale * 1.2f, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);

        chaserManager.CallChaser();
        sliderImage.color = Color.red;

        yield return new WaitForSeconds(60);

        sliderTween.Kill();
        sliderImage.transform.localScale = originalScale;
        callValue = 0;
        sliderImage.fillAmount = Mathf.Clamp01(callValue / 100f);
        chaserManager.RecallChaser();
        sliderImage.color = fillColor;
    }



    public void AddValue(int value)
    {
        callValue += value;
        sliderImage.fillAmount = Mathf.Clamp01(callValue / 100f);
    }
    public void RemoveValue(int value)
    {
        callValue  -= value;
        sliderImage.fillAmount = Mathf.Clamp01(callValue / 100f);
    }
}
