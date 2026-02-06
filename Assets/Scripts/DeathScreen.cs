using UnityEngine;

public class DeathScreen : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void ActivateDS()
    {
        animator.SetTrigger("Death");
    }

    public void MenuButton()
    {
        SceneLoader.Instance.LoadScene("Menu");
    }
    public void RestartButton()
    {
        SceneLoader.Instance.LoadScene("Game");
    }
}
