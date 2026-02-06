using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    [Header("Director")]
    [SerializeField] private PlayableDirector director;

    [Header("Cutscenes")]
    [SerializeField] private PlayableAsset[] cutscenes;

    private PlayerController playerController;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    public void PlayCutscene(int index)
    {
        if (index < 0 || index >= cutscenes.Length)
        {
            Debug.LogError("Cutscene index out of range");
            return;
        }

        director.playableAsset = cutscenes[index];
        director.Play();
    }

    public void OnCutsceneStart()
    {
        playerController.canMove = false;
        playerController.canRotate = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void OnCutsceneEnd()
    {
        playerController.canMove = true;
        playerController.canRotate = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
