using UnityEngine;

public class Door : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    [Header("One-Side Settings")]
    public Transform frontDirection; // The side from which the door can be opened
    public bool onlyOpenFromFront = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (frontDirection == null)
            frontDirection = transform; // default to door's forward direction
    }

    public bool CanOpenFrom(Vector3 playerPosition)
    {
        if (!onlyOpenFromFront) return true;

        Vector3 toPlayer = playerPosition - transform.position;
        float dot = Vector3.Dot(frontDirection.forward, toPlayer);
        return dot > 0f; // Only open if player is in front
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
        animator.SetBool("isOpen", isOpen);
    }
}
