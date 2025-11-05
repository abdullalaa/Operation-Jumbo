using UnityEngine;

public class endLevelTrigger : MonoBehaviour
{
    public LayerMask playerLayer;
    public bool loadNextLevel = false;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other);
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered next level trigger");
            loadNextLevel = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (loadNextLevel)
        {
            Debug.Log("Player entered next level");
            loadNextLevel = false;
        }
    }
}
