/*
 * Script to attach plant to player, and move the player to "hidden" layer
 */


using UnityEngine;

public class PlantController : MonoBehaviour
{
    private Animator animator;

    private string playerTag = "Medium";
    private string currentPlayerTag;

    private bool isAttached = false;

    private bool playerInRange = false;
    private Transform playerTransform;

    [SerializeField]
    private float offsetUnit = 0.2f;

    void Start()
    {
        animator = GetComponent<Animator>(); // Wiggle animation to attract attention
    }

    void Update()
    {
        if (!isAttached && playerInRange) // When plant is not attached & player is within collider
        {
            animator?.SetBool("PlantWiggle", true); // Play animation to attract attention

            if (Input.GetKeyDown(KeyCode.E)) // Attach player when e is pressed
            {
                AttachToPlayer();
            }
        }
        else // When plant is attached
        {
            // Drop plant when e is pressed or tag has changed (going through gate)
            if (Input.GetKeyDown(KeyCode.E) || (playerTransform != null && playerTransform.tag != currentPlayerTag))
            {
                DetachFromPlayer();
            }

            // Follow player at attach position
            if (playerTransform != null)
            {
                transform.position = GetAttachPosition();
                transform.rotation = playerTransform.rotation;
            }
        }
    }

    private Vector3 GetAttachPosition()
    {
        if (playerTransform == null) return transform.position;

        Collider playerCollider = playerTransform.GetComponent<Collider>();
        if (playerCollider == null) return playerTransform.position;

        // Get the forward direction of the player
        Vector3 forward = playerTransform.forward;

        // Calculate the edge offset
        Vector3 offset = forward * (playerCollider.bounds.extents.z + offsetUnit); // 0.2 units in front

        // Attach at the edge in front of the player
        return playerTransform.position + offset;
    }


    // Attach plant to player
    private void AttachToPlayer()
    {
        isAttached = true;
        animator?.SetBool("PlantWiggle", false); // Stop playing animation

        currentPlayerTag = playerTransform.tag; // Store tag, to see if it changed through gate

        playerTransform.gameObject.layer = LayerMask.NameToLayer("Hidden"); // Move player to hidden layer
    }

    // Detach plant from player
    private void DetachFromPlayer()
    {
        playerTransform.gameObject.layer = LayerMask.NameToLayer("Player"); // Restore player layer
        isAttached = false;
        playerInRange = false;
        playerTransform = null; // Reset attach point
        animator?.SetBool("PlantWiggle", false); 
    }

    // Check if player is in range and if it is medium player to use the attach position from player
    private void OnTriggerEnter(Collider other)
    {
        if (!isAttached && other.CompareTag(playerTag))
        {
            playerInRange = true;
            playerTransform = other.transform;
        }
    }

    // Reset everything when exiting collider
    private void OnTriggerExit(Collider other)
    {
        if (!isAttached && other.CompareTag(playerTag))
        {
            playerInRange = false;
            animator?.SetBool("PlantWiggle", false);
            playerTransform = null;
        }
    }
}
