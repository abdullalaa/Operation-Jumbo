using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    // Attach to main camera
    
    public GameObject player;
    public Vector3 offset = new Vector3(-3.3f, 8.5f, 0f);
    public float rotateX = 60f;
    public float rotateY = 90f;
    //private float turnSpeed = 10.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        float swap = -1;
        //float rot = player.transform.rotation.eulerAngles.y;
        //if (rot > 180)
        //{
        //    swap = -1;
        //}

        transform.position = player.transform.position + new Vector3(swap * offset.x, offset.y, offset.z);
        transform.rotation = Quaternion.identity;
        transform.RotateAround(transform.position, transform.up, swap * rotateY);
        transform.RotateAround(transform.position, transform.right, rotateX);
    }

    private void FixedUpdate()
    {
        //float rotateHorizontal = Input.GetAxis("Mouse X");
        //float rotateVertical = Input.GetAxis("Mouse Y");

        //transform.RotateAround(player.transform.position, Vector3.up, rotateHorizontal * turnSpeed);
        //transform.RotateAround(Vector3.zero, transform.right, rotateVertical * turnSpeed);
    }
}
