using UnityEngine;

public class Note : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject canvas;
    
    private SoundPlayer soundPlayer;
    private void Start()
    {
        soundPlayer = GetComponent<SoundPlayer>();   
    }

    void Update()
    {
        if (canvas.activeSelf && Input.GetKeyDown(KeyCode.Mouse0))
        {
            canvas.SetActive(false);
        }
    }

    public void DoSomething()
    {
        soundPlayer.PlaySound(0);
        canvas.SetActive(true);
    }
}
