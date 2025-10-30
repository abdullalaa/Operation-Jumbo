using UnityEngine;

public class KnockOutGuards : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Large"))
        {
            Destroy(gameObject);
            Debug.Log("Knocked Out " + gameObject.name);
        }


    }
}