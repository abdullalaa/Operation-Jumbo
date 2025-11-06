using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // Attach to player object

    public float avgSpeed = 5f; // Speed that is used for medium state
    public float turnSpeed = 150.0f;
    private float speed; // Speed used in current state (changed in code)

    private float horizontalInput; // Input given by mouse to turn the player and camera
    public float forwardInput; // Input to move the player forward
    private bool verticalInput; // Input to move the player up (when floating)
    public float sideInput; // Input to move the player to the side
    
    public bool mouseToTurn = true; // Indicates wheter to activate player and camera rotation with the mouse

    public float maxHeight = 4.0f; // Highest point the player can be when floating
    public float floatForce = 0.5f; // The force with which the player is moved when floating
    private bool isLowEnough; // Tests whether the player is not above limits
    private bool floating = false; // True when in state "Float"

    private Rigidbody rb; // Rigidbody component of the player
    private BoxCollider box; // Boxcollider component of the player
    private GameObject elephant; // Visual model connected to the player
    private Animator animator; // Animator that is connected to the elephant, enabling its animations

    private bool initialized = false; // Indicates whether the player has been initialized, flag for camera positioning

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        box = GetComponent<BoxCollider>();
        elephant = GameObject.Find("Elephant");
        animator = elephant.GetComponent<Animator>();
        speed = avgSpeed;
        initialized = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!GameManager.instance.LimitPlayerByWire())
        {
            forwardInput = -0.01f;
            //sideInput = 0;
            return;
        }
        if (GameManager.instance.gameOver)
        {
            animator.SetBool("isWalking", false);
            return;
        }
        if (mouseToTurn)
        {
            horizontalInput = Input.GetAxis("Mouse X");
            // Rotates the character based on horizontal input (mouse movement)
            transform.Rotate(Vector3.up, turnSpeed * horizontalInput * Time.deltaTime);

        }
        else
        {
            // Reset rotation if mouse to turn is disabled
            transform.rotation = Quaternion.identity;
            transform.Rotate(0, -90, 0);
        }

        forwardInput = Input.GetAxis("Vertical");
        verticalInput = Input.GetKey(KeyCode.Space);
        sideInput = Input.GetAxis("Horizontal");

        if (forwardInput == 0 || floating)
        {
            // Deactivate walking animation when not moving forward or when floating
            animator.SetBool("isWalking", false);
        }
        else
        {
            // Activate walking animation when moving forward
            animator.SetBool("isWalking", true);
        }

        if (floating)
        {
            // Apply impulse forces to move the player when floating based on input given by the keyboard
            rb.AddForce(transform.forward * (forwardInput / 20), ForceMode.Impulse);
            rb.AddForce(transform.right * (sideInput / 20), ForceMode.Impulse);

            isLowEnough = transform.position.y < maxHeight;

            // While space is pressed and player is low enough, float up using force
            if (verticalInput && isLowEnough)
            {
                rb.AddForce(Vector3.up * floatForce, ForceMode.Impulse);
            }

            else if (!isLowEnough)
            {
                // If player exceeds max height, set position to max height and apply small downward force
                transform.position = new Vector3(transform.position.x, maxHeight, transform.position.z);
                rb.AddForce(Vector3.down * 0.5f, ForceMode.Impulse);
            }

            else if (transform.position.y < 1.5)
            {
                // If player is below minimum height, set position to minimum height and apply small upward force
                transform.position = new Vector3(transform.position.x, 1.5f, transform.position.z);
                rb.AddForce(Vector3.up * 0.5f, ForceMode.Impulse);
            }

        }
        
        else
        {
            // Move the player directly based on input given by the keyboard when not floating
            transform.Translate(Vector3.forward * Time.deltaTime * speed * forwardInput);
            transform.Translate(Vector3.right * Time.deltaTime * speed * sideInput);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7) // Layer 7 is the layer with all the gates
        {
            Vector3 direction = (transform.position - other.transform.position).normalized;
            float dot = Vector3.Dot(other.transform.up, direction);
            if (dot < 0f) // Use the dot product to determine if we exit through the back side of the gate
            {
                switch (other.gameObject.tag)
                {
                    case "Small":
                        floating = false;
                        gameObject.tag = "Small";
                        box.size = new Vector3(1, 2, 1);
                        box.center = new Vector3(0, 0, 0);
                        gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Scale down the player
                        speed = 1.5f * avgSpeed; // Speed up the player
                        animator.SetFloat("speed", 2.0f); // Speed up the walking animation
                        elephant.transform.localRotation = Quaternion.identity; // Reset rotation of the elephant model
                        elephant.transform.localPosition = new Vector3(0, -0.97f, 0); // Reset position of the elephant model
                        break;
                    case "Medium":
                        floating = false;
                        gameObject.tag = "Medium";
                        box.size = new Vector3(1, 2, 1);
                        box.center = new Vector3(0, 0, 0);
                        gameObject.transform.localScale = Vector3.one; // Reset scale of the player
                        speed = avgSpeed; // Reset speed of the player
                        animator.SetFloat("speed", 1.5f); // Reset speed of the walking animation
                        elephant.transform.localRotation = Quaternion.identity; // Reset rotation of the elephant model
                        elephant.transform.localPosition = new Vector3(0, -0.97f, 0); // Reset position of the elephant model
                        break;
                    case "Large":
                        floating = false;
                        gameObject.tag = "Large";
                        box.size = new Vector3(1, 2, 1);
                        box.center = new Vector3(0, 0, 0);
                        gameObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f); // Scale up the player
                        speed = 0.5f * avgSpeed; // Slow down the player
                        animator.SetFloat("speed", 1.0f); // Slow down the walking animation
                        elephant.transform.localRotation = Quaternion.identity; // Reset rotation of the elephant model
                        elephant.transform.localPosition = new Vector3(0, -0.97f, 0); // Reset position of the elephant model
                        break;
                    case "Float":
                        floating = true;
                        gameObject.tag = "Float";
                        box.size = new Vector3(1, 1, 2);
                        box.center = new Vector3(0, 0, 1);
                        gameObject.transform.localScale = Vector3.one; // Reset scale of the player
                        speed = avgSpeed; // Reset speed of the player
                        elephant.transform.localRotation = Quaternion.identity; // Reset rotation of the elephant model
                        elephant.transform.Rotate(90, 0, 0); // Rotate the elephant model to a floating position (horizontal)
                        elephant.transform.localPosition = new Vector3(0, 0.03f, 0); // Adjust position of the elephant model for floating
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public bool isInitialized()
    {
        return initialized;
    }
}
