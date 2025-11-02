using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //singletone pattern
    public static GameManager instance;

    [Header ("Hint Message")]
    [SerializeField] GameObject endPointMSG;

    [Header("Gameplay Reference")]
    [SerializeField] Transform player;
    [SerializeField] Wire wire;

    void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        LimitPlayerByWire();
    }

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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnWireConnected()
    {
        Debug.Log("Wire Connected!");
    }
    public void ShowHint(bool isShow)
    {
        endPointMSG.SetActive(isShow);
    }
}
