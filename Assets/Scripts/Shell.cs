using UnityEngine;

public class Shell : MonoBehaviour
{
    private SoundPlayer soundPlayer;

    private Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        soundPlayer = GetComponent<SoundPlayer>();

        // Сила выброса гильзы
        Vector3 force = transform.right * Random.Range(2f, 4f) +
                        transform.up * Random.Range(1f, 2f);

        rb.AddForce(force, ForceMode.Impulse);

        // Немного вращения
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);

        Destroy(gameObject, 5f);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            soundPlayer.PlaySound(0);
        }

    }
}
