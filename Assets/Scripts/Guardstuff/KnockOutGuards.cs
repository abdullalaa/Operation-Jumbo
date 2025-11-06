using UnityEngine;

public class KnockOutGuards : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip punch;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Large"))
        {
            Destroy(gameObject);
            audioSource.PlayOneShot(punch);
            Debug.Log("Knocked Out " + gameObject.name);
        }


    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Large"))
        {
            Destroy(gameObject);
            Debug.Log("Knocked Out " + gameObject.name);
        }
    }
}