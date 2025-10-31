using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Attach to player object

    [SerializeField] Wire wire;

    public float speed = 10.0f;
    public float turnSpeed = 150.0f;
    private float horizontalInput;
    private float forwardInput;
    private bool verticalInput;
    private float sideInput;

    private bool isLowEnough;
    public float maxHeight = 4.0f;
    public float floatForce = 3.0f;

    private Rigidbody rb;

    private bool floating = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        rb.isKinematic = false;

        //horizontalInput = Input.GetAxis("Horizontal");
        horizontalInput = Input.GetAxis("Mouse X");
        forwardInput = Input.GetAxis("Vertical");
        verticalInput = Input.GetKey(KeyCode.Space);
        sideInput = Input.GetAxis("Horizontal");

        if (floating)
        {
            //rb.isKinematic = false;
            //rb.angularVelocity = Vector3.zero;
            //rb.angularVelocity = Vector3.zero;
            rb.AddForce(transform.forward * (forwardInput / 20), ForceMode.Impulse);
            rb.AddForce(transform.right * (sideInput / 20), ForceMode.Impulse);

            isLowEnough = transform.position.y < maxHeight;

            // While space is pressed and player is low enough, float up
            if (verticalInput && isLowEnough)
            {
                rb.AddForce(Vector3.up * floatForce);
            }

            else if (!isLowEnough)
            {
                transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
                rb.AddForce(Vector3.down * 0.5f, ForceMode.Impulse);
            }

            else if (transform.position.y < 1)
            {
                transform.position = new Vector3(transform.position.x, 1.0f, transform.position.z);
                rb.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
            }

        }
        //else if (!floating && !rb.isKinematic)
        //{
        //    //rb.isKinematic = true;
        //}
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
            transform.Translate(Vector3.right * Time.deltaTime * speed * sideInput);

        }

        // Rotates the character based on horizontal input (mouse movement)
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);

        if (!floating)
        {
            // Move the character forward based on vertical input

        }

        //transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);
    }

    private void FixedUpdate()
    {

        if (floating && !rb.isKinematic)
        {
            

        }

        if (wire != null)
        {
            Vector3 anchorPos = wire.startTransform.position;
            float maxDis = wire.totalLength;

            Vector3 dir = rb.position - anchorPos;
            float dist = dir.magnitude;

            if (dist > maxDis)
            {
                dir = dir.normalized * maxDis;
                Vector3 newPos = anchorPos + dir;

                if (rb.isKinematic)
                {
                    transform.position = newPos;
                }
                else
                {
                    rb.MovePosition(newPos);
                }
            }

        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            Debug.Log("collision");
            Vector3 direction = (transform.position - other.transform.position).normalized;
            float dot = Vector3.Dot(other.transform.up, direction);
            if (dot < 0f)
            {
                Debug.Log("front");
                switch (other.gameObject.tag)
                {
                    case "Small":
                        Debug.Log("s");
                        floating = false;
                        gameObject.tag = "Small";
                        gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        gameObject.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                        speed = 15.0f;
                        break;
                    case "Medium":
                        Debug.Log("m");
                        floating = false;
                        gameObject.tag = "Medium";
                        gameObject.transform.localScale = Vector3.one;
                        gameObject.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                        speed = 10.0f;
                        break;
                    case "Large":
                        Debug.Log("l");
                        floating = false;
                        gameObject.tag = "Large";
                        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        gameObject.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
                        speed = 5.0f;
                        break;
                    case "Float":
                        Debug.Log("f");
                        floating = true;
                        gameObject.tag = "Float";
                        gameObject.transform.localScale = Vector3.one;
                        gameObject.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                        speed = 10.0f;
                        break;
                    default:
                        break;
                }
                transform.Translate(Vector3.forward * forwardInput * 3f);
            }
        }
    }

    public float getHorizontal()
    {
        return turnSpeed * horizontalInput * Time.deltaTime;
    }
}
