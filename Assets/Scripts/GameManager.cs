using System;
//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditorInternal;

public class GameManager : MonoBehaviour
{
    //singletone instance for global access
    public static GameManager instance;

    [Header ("Hint Message")]
    [SerializeField] GameObject endPointMSG;

    [Header("Gameplay Reference")]
    [SerializeField] Transform player;
    // reference to wire system
    [SerializeField] public PlugWire wire;
    [SerializeField] InteractionWEndPoint interact;

    [Header ("Respawn Menu")]
    public Canvas respawnMenu;
    public Button respawnButton;

    private bool isRespawnMenuActive = false;
    public bool gameOver = false;

    void Awake()
    {
        // assign global instance
        instance = this;

        respawnMenu.gameObject.SetActive(false); // Hide at start
        respawnButton.onClick.AddListener(OnRespawnButtonClicked);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    // physics update
    void FixedUpdate()
    {
        // it uses Rigidbody movement
        LimitPlayerByWire();
    }

    // prevent the player form moving further than the total wire length
    void LimitPlayerByWire()
    {
        if (wire == null || player == null) return;
        if (wire.isLockedToEndPoint) return;

        float maxLen = wire.GetMaxLength();
        float realLen = wire.CalcRealLength();
        if (realLen <= maxLen) return;


        Vector3 startPos = wire.startTransform.position;
        Vector3 dir = (player.position - startPos).normalized;
        Vector3 newPos = startPos + dir*maxLen;

        player.position = newPos;
        Rigidbody rb = player.GetComponent<Rigidbody>();

        if(rb != null )
        {
            rb.MovePosition(newPos);
            rb.linearVelocity = Vector3.zero;
        }
        else
        {
            player.position = newPos;
        }
    }
    public void ConnectedToEndPoint(Vector3 pos)
    {
        wire.LockTo(pos);
    }

    public void ResetLevel()
    {
        if(wire != null) wire.ResetWire();
        wire.Unlock();
        if (interact != null) interact.ResetConnection();

        Debug.Log("Level reset done");
    }



    // called when wire successfully connects to endpoint
    public void OnWireConnected()
    {
        Debug.Log("Wire Connected!");
    }

    // show or hide "Press F Key" UI hint for endpoint
    public void ShowHint(bool isShow)
    {
        endPointMSG.SetActive(isShow);
    }

    private void OnRespawnButtonClicked()
    {

        ReloadCurrentScene();

        respawnMenu.gameObject.SetActive(false);
        isRespawnMenuActive = false;
        gameOver = false;
    }

    // Called when player is spotted
    public void ShowRespawnMenu(GameObject player)
    {

        if (isRespawnMenuActive) return; // Prevent multiple calls
        gameOver = true;

        respawnMenu.gameObject.SetActive(true);
        isRespawnMenuActive = true;
    }

    public void ReloadCurrentScene()
    {
        // Get the active scene
        Scene currentScene = SceneManager.GetActiveScene();

        // Reload the active scene
        SceneManager.LoadScene(currentScene.name);
    }
}
