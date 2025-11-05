using UnityEngine;

public class PlugFollow : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private Transform playerTransform;

    [Header("Settings")]

    [SerializeField]
    private float posOffset = 0.7f;
    [SerializeField]
    private Quaternion rotOffset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.rotation = rotOffset;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform == null) return;

        //Collider playerCollider = playerTransform.GetComponent<Collider>();
        //if (playerCollider == null) return;

        // Get the forward direction of the player
        Vector3 forward = playerTransform.forward;

        // Calculate the edge offset
        Vector3 offset = -forward * posOffset;

        // Attach at the edge in front of the player
        Vector3 attachPos = playerTransform.position + offset;

        transform.position = attachPos;
        transform.rotation = playerTransform.rotation * rotOffset;

    }
}
