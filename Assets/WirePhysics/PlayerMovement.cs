using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public Wire wire;

    private Rigidbody rb;
    private float moveX, moveZ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // A, D
        moveX = Input.GetAxis("Horizontal");
        // W, S
        moveZ = Input.GetAxis("Vertical");

        

    }
    private void FixedUpdate()
    {
        Vector3 move = new Vector3(moveX, 0f, moveZ) * moveSpeed;
        Vector3 newPos = rb.position + move * Time.deltaTime;


        if (wire != null)
        {
            Vector3 anchorPos = wire.startTransform.position;
            float maxDis = wire.totalLength;

            Vector3 dir = newPos - anchorPos;
            float dist = dir.magnitude;

            if(dist > maxDis)
            {
                dir = dir.normalized * maxDis;
                newPos = anchorPos + dir;
            }

        }

        rb.MovePosition(newPos);
    }
}
