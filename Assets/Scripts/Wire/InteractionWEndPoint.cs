using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionWEndPoint : MonoBehaviour
{
    [Header("Wire Interaction Settings")]
    [SerializeField] Transform wireEnd;

    //waittimer
    private float waitTime = 0.5f;
    private float time = 0;
    // check using script instead of tag
    private PlugEndPoint currentEndPoint;
    // check player is inside trigger range and able to press F
    private bool canPress = false;
    // check wire is already connected or not
    public bool isConnected = false;
    // reference to GameManager
    private GameManager gm;
    

    void Update()
    {
        if (GameManager.instance.gameOver) 
                {
            return;
                }

        // find GameManager reference first
        if (gm == null) gm = gm ?? GameManager.instance;

        // player presses F to connect wire to endpoint
        if (canPress && Input.GetKeyDown(KeyCode.F))
        {
            if(currentEndPoint != null)
            {
               

              gm.ConnectedToEndPoint(currentEndPoint.transform.position);

              isConnected = true;
              gm.OnWireConnected();
              gm.ShowHint(false);
               

            }
        }
    }

    // Check if player is within the endpoint range
    private void OnTriggerEnter(Collider other)
    {
        PlugEndPoint endpoint = other.GetComponent<PlugEndPoint>();
        if (endpoint != null) { 
            canPress = true;
            currentEndPoint = endpoint;
            gm.ShowHint(true);
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
            gm.ShowHint(false);
        }
    }

    public void ResetConnection()
    {
        isConnected = false;
        currentEndPoint = null;
        canPress = false;
    }
}
