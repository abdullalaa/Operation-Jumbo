using UnityEngine;

public class Level3trigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other != CompareTag("Float"))
        {
            gameObject.GetComponent<BoxCollider>().enabled = false;
        }

    }
}
