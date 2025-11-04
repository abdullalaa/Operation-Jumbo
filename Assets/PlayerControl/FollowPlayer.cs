using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class FollowPlayer : MonoBehaviour
{
    // Attach to main camera
    
    public GameObject player;
    public Vector3 offset = new Vector3(0f, 8.5f, -3.3f);
    public float rotateX = 60f;
    public float rotateY = -90f;
    //private float rotateHorizontal;
    private float turnSpeed = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //transform.rotation = Quaternion.identity;
        //transform.RotateAround(transform.position, transform.up, rotateY);
        //transform.RotateAround(Vector3.zero, transform.right, rotateX);

        transform.rotation = Quaternion.Euler(rotateX, rotateY, 0f);

        StartCoroutine(LateStart());
    }

    IEnumerator LateStart()
    {
        yield return new WaitUntil(() => player != null && player.GetComponent<PlayerControl>().isInitialized());
        transform.position = player.transform.position + offset;
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //float swap = -1;
        //float rot = player.transform.rotation.eulerAngles.y;
        //if (rot > 180)
        //{
        //    swap = -1;
        //}

        //rotateHorizontal = Input.GetAxis("Mouse X");
        //transform.position = player.transform.position + new Vector3(swap * offset.x, offset.y, offset.z);
        //transform.rotation = Quaternion.identity;
        //transform.RotateAround(transform.position, transform.up, swap * rotateY);
        //transform.RotateAround(transform.position, transform.right, rotateX);

        if (player == null) return;

        // PlayerControl already rotates the player with horizontal mouse input.
        // Camera follows player's rotation and position using a local offset.
        Vector3 desiredPosition = player.transform.position + player.transform.rotation * offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Mathf.Clamp01(Time.deltaTime * turnSpeed));

        Quaternion desiredRotation = Quaternion.Euler(rotateX, player.transform.eulerAngles.y, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, Mathf.Clamp01(Time.deltaTime * turnSpeed));
    }

    //private void FixedUpdate()
    //{
    //    float rotateHorizontal = Input.GetAxis("Mouse X");
    //    //float rotateVertical = Input.GetAxis("Mouse Y");

    //    transform.RotateAround(player.transform.position, Vector3.up, rotateHorizontal * turnSpeed);
    //    transform.position = new Vector3(transform.position.x, player.transform.position.y + offset.y, transform.position.z);

    //    //transform.RotateAround(Vector3.zero, transform.right, rotateVertical * turnSpeed);
    //}
}
