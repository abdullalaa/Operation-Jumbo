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
    [SerializeField] Wire wire;
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

        Vector3 anchorPos = wire.startTransform.position;
        float maxDis = wire.GetRealTotalLength();

        Vector3 dir = player.position - anchorPos;
        float dist = dir.magnitude;

        if (dist > maxDis)
        {
            dir = dir.normalized * maxDis;
            Vector3 newPos = anchorPos + dir;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if(rb != null && rb.isKinematic == false)
            {
                rb.MovePosition(newPos);
            }
            else
            {
                player.position = newPos;
            }
        }
    }

    public void ResetLevel()
    {
        if(wire != null) wire.ResetWire();
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
