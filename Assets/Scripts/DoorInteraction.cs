using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactDistance = 2f;
    public KeyCode interactKey = KeyCode.E;
    public Transform rayOrigin; // where to cast the ray from (e.g. player chest or head)
    public bool showDebugRay = true;

    private void Start()
    {
        // fallback if not assigned
        if (rayOrigin == null)
            rayOrigin = transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            Ray ray = new Ray(rayOrigin.position, rayOrigin.forward);
            if (showDebugRay)
                Debug.DrawRay(ray.origin, ray.direction * interactDistance, Color.yellow, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    Door door = hit.collider.GetComponentInParent<Door>();
                    if (door != null)
                    {
                        if (door.CanOpenFrom(transform.position))
                        {
                            door.ToggleDoor();
                        }
                        else
                        {
                            Debug.Log("Can't open from this side!");
                        }
                    }
                }
            }
        }
    }
}
