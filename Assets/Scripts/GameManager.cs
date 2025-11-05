using System;
using UnityEngine;

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

    void Awake()
    {
        // assign global instance
        instance = this;
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

        float maxLen = wire.GetMaxLength();
        float realLen = wire.CalcRealLength();
        if (realLen <= maxLen) return;


        Vector3 startPos = wire.startTransform.position;
        Vector3 dir = (player.position - startPos).normalized;
        Vector3 newPos = startPos + dir*maxLen;

        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.MovePosition(newPos);
        }
        else
        {
            player.position = newPos;
        }
    }

    public void ResetLevel()
    {
        //if(wire != null) wire.ResetWire();
        if(interact != null) interact.ResetConnection();

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
}
