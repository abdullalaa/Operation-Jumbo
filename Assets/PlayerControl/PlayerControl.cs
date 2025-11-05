using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Attach to player object
    public float avgSpeed = 5f;

    private float speed = 10.0f;
    public float turnSpeed = 150.0f;

    private float horizontalInput;
    public float forwardInput;
    private bool verticalInput;
    public float sideInput;

    public bool mouseToTurn = false;

    private bool isLowEnough;
    public float maxHeight = 4.0f;
    public float floatForce = 3.0f;

    private Rigidbody rb;

    private bool floating = false;

    private bool initialized = false;

    private Animator animator;

    private GameObject elephant;
    private BoxCollider box;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        initialized = true;
        elephant = GameObject.Find("Elephant");
        animator = elephant.GetComponent<Animator>();
        box = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.isKinematic = false;

        //horizontalInput = Input.GetAxis("Horizontal");
        if (mouseToTurn)
        {
            horizontalInput = Input.GetAxis("Mouse X");
            // Rotates the character based on horizontal input (mouse movement)
            transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);

        }
        else
        {
            transform.rotation = Quaternion.identity;
            transform.Rotate(0, -90, 0);
        }
        forwardInput = Input.GetAxis("Vertical");
        verticalInput = Input.GetKey(KeyCode.Space);
        sideInput = Input.GetAxis("Horizontal");

        if (forwardInput == 0 || floating)
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }

        if (floating)
        {
            animator.SetBool("isWalking", false);
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

            else if (transform.position.y < 1.5)
            {
                transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
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

        

        if (!floating)
        {
            // Move the character forward based on vertical input

        }

        //transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput);
    }

    private void Update()
    {

        if (floating && !rb.isKinematic)
        {
            

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
                        box.size = new Vector3(1, 2, 1);
                        box.center = new Vector3(0, 0, 0);
                        gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                        //gameObject.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);
                        speed = 1.5f * avgSpeed;
                        animator.SetFloat("speed", 2.0f);
                        elephant.transform.localRotation = Quaternion.identity;
                        break;
                    case "Medium":
                        Debug.Log("m");
                        floating = false;
                        gameObject.tag = "Medium";
                        box.size = new Vector3(1, 2, 1);
                        box.center = new Vector3(0, 0, 0);
                        gameObject.transform.localScale = Vector3.one;
                        //gameObject.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                        speed = avgSpeed;
                        animator.SetFloat("speed", 1.5f);
                        elephant.transform.localRotation = Quaternion.identity;
                        break;
                    case "Large":
                        Debug.Log("l");
                        floating = false;
                        gameObject.tag = "Large";
                        box.size = new Vector3(1, 2, 1);
                        box.center = new Vector3(0, 0, 0);
                        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        //gameObject.transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
                        speed = 0.5f * avgSpeed;
                        animator.SetFloat("speed", 1.0f);
                        elephant.transform.localRotation = Quaternion.identity;
                        break;
                    case "Float":
                        Debug.Log("f");
                        floating = true;
                        gameObject.tag = "Float";
                        box.size = new Vector3(1, 1, 2);
                        box.center = new Vector3(0, 0, 1);
                        gameObject.transform.localScale = Vector3.one;
                        //gameObject.transform.position = new Vector3(transform.position.x, 1, transform.position.z);
                        speed = avgSpeed;
                        elephant.transform.localRotation = Quaternion.identity;
                        elephant.transform.Rotate(90, 0, 0);
                        break;
                    default:
                        break;
                }
                //transform.Translate(Vector3.forward * forwardInput * 3f);
            }
        }
    }

    public float getHorizontal()
    {
        return turnSpeed * horizontalInput * Time.deltaTime;
    }

    public bool isInitialized()
    {
        return initialized;
    }
}
