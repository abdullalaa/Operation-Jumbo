using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Attach to player object

    public float speed = 10.0f;
    public float turnSpeed = 150.0f;
    private float horizontalInput;
    private float forwardInput;
    private bool verticalInput;

    private bool isLowEnough;
    public float maxHeight = 5.0f;
    public float floatForce = 50.0f;

    private Rigidbody rb;

    private bool floating = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //horizontalInput = Input.GetAxis("Horizontal");
        horizontalInput = Input.GetAxis("Mouse X");
        forwardInput = Input.GetAxis("Vertical");
        verticalInput = Input.GetKey(KeyCode.Space);

        if (floating)
        {
            rb.AddForce(transform.forward * forwardInput * 0.1f, ForceMode.Impulse);

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
            
            else if (transform.position.y < 2)
            {
                transform.position = new Vector3(transform.position.x, 2.0f, transform.position.z);
                rb.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
            }
        }
        else
        {
            // Move the character forward based on vertical input
            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
        }
        // Rotates the character based on horizontal input (mouse movement)
        transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);
        //transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            Debug.Log("collision");
            Vector3 direction = (other.transform.position - transform.position).normalized;
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
