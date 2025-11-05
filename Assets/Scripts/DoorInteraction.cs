using UnityEngine;

public class DoorInteraction : MonoBehaviour
{
    public float interactDistance = 3f;
    public KeyCode interactKey = KeyCode.E;
    private Camera playerCam;

    void Start()
    {
        playerCam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            // Cast a ray forward from the player's camera
            Ray ray = new Ray(playerCam.transform.position, playerCam.transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance))
            {
                if (hit.collider.CompareTag("Door"))
                {
                    Door door = hit.collider.GetComponent<Door>();
                    if (door != null && door.CanOpenFrom(transform.position))
                    {
                        door.ToggleDoor();
                    }
                }
            }
        }
    }
}
