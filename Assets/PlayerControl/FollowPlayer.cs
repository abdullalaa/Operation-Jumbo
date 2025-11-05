using System.Collections;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Attach to camera
    
    public GameObject player; // The player that is followed by the camera

    private Vector3 offset = new Vector3(0f, 8.5f, -3.3f); // The distance from the player to the camera
    private float rotateX = 60f; // The rotation along the X-axis of the camera to make sure it faces the player
    private float rotateY = 90f; // The rotation along the Y-axis of the camera to make sure it faces the player

    private float turnSpeed = 10.0f; // To ensure smooth movement of the camera

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initially rotate the camera as such so that it faces the correct way
        transform.rotation = Quaternion.Euler(rotateX, rotateY, 0f);

        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        // Wait until player is initialized before setting camera position
        yield return new WaitUntil(() => player != null && player.GetComponent<PlayerControl>().isInitialized());
        
        // Set the camera to the position of the player with an offset
        transform.position = player.transform.position + offset;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null) return;

        // PlayerControl already rotates the player with horizontal mouse input.
        // Camera follows player's rotation and position using a local offset.
        Vector3 desiredPosition = player.transform.position + player.transform.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Mathf.Clamp01(Time.deltaTime * turnSpeed));

        Quaternion desiredRotation = Quaternion.Euler(rotateX, player.transform.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Mathf.Clamp01(Time.deltaTime * turnSpeed));
    }
}
