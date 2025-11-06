using UnityEngine;
using UnityEngine.SceneManagement;

public class EasterEggTrigger : MonoBehaviour
{
    public GameObject message;
    int playerLayer;
    

    void Start()
    {
        playerLayer = LayerMask.NameToLayer("Player");
        message.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            message.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == playerLayer)
        {
            message.SetActive(false);
        }
    }
}
