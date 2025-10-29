using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionWEndPoint : MonoBehaviour
{
    [Header("Wire Interaction Settings")]
    [SerializeField] Transform wireEnd;
    [SerializeField] GameObject alertMessage;

    // check using script instead of tag
    private PlugEndPoint currentEndPoint;
    private bool canPress = false;
    private bool isConnected = false;


    void Update()
    {
        if(canPress && Input.GetKeyDown(KeyCode.F))
        {
            if(wireEnd != null && currentEndPoint != null)
            {
                // if these conditions is satisfied, change wireEnd position
                wireEnd.position = currentEndPoint.transform.position;
                isConnected = true;
            }
        }

        if(isConnected && wireEnd != null && currentEndPoint != null)
        {
            wireEnd.position = currentEndPoint.transform.position;
        }
    }

    // Check if player is within the endpoint range
    private void OnTriggerEnter(Collider other)
    {
        PlugEndPoint endpoint = other.GetComponent<PlugEndPoint>();
        if (endpoint != null) { 
            canPress = true;
            currentEndPoint = endpoint;
            alertMessage.SetActive(true);
        }
    }

    // Check if player is out of endpoint range
    private void OnTriggerExit(Collider other)
    {
        PlugEndPoint endpoint = other.GetComponent<PlugEndPoint>();
        if (endpoint != null && !isConnected)
        {
            canPress = false;
            currentEndPoint = null;
            alertMessage.SetActive(false);
        }
    }
}
