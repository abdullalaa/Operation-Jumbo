using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class PlugWire : MonoBehaviour
{

    [Header("Reference")]
    [SerializeField] public Transform startTransform;
    [SerializeField] public Transform endTransform;
    [SerializeField] public Transform segments;
    [SerializeField] Transform routeParent;

    [SerializeField] GameObject player;

    [Header("Settings")]
    // sphere radius
    [SerializeField] float radius = 0.5f;
    // start count
    [SerializeField] int initialSeg = 4;

    [Header("Models")]
    [SerializeField] GameObject wirePrefab;
    [SerializeField] GameObject plugPrefab;

    [Header("Plug")]
    public bool isLockedToEndPoint = false;
    public Vector3 lockedPosition;

    List<Transform> segs = new List<Transform> ();
    float spacing;
    float maxRouteLength;
    int segIndex = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // remove all seg
        for (int i = segments.childCount - 1; i >= 0; i--)
        {
            Destroy(segments.GetChild(i).gameObject);
        }

        segs.Clear();

        spacing = radius * 2f;
        maxRouteLength = CalcRouteLentgh();

        // create first fixed segment == start
        var first = CreateSeg(startTransform.position);
        segs.Add(first);
        first.GetComponent<Rigidbody>().isKinematic = true;
        //segs[0].position = startTransform.position;

        // create 3 initial spheres
        for (int i = 1; i < initialSeg-1; i++) 
        {
            segs.Add(CreateSeg(startTransform.position));

        }
        segs.Add(CreateSeg(startTransform.position, true));

        //initial spread
        Vector3 dir = (player.transform.position - startTransform.position).normalized;

        for(int i = 1; i<segs.Count; i++)
        {
            segs[i].position = startTransform.position + dir * spacing * i;
        }

        for(int i = 1; i < segs.Count-1; i++)
        {
            segs[i].GetComponent<ConfigurableJoint>().connectedBody = segs[i-1].GetComponent<Rigidbody>();
        }
        Destroy(segs[segs.Count - 1].GetComponent<ConfigurableJoint>());
    }

    Transform CreateSeg(Vector3 position, bool isPlug = false)
    {
        GameObject prefab = isPlug ? plugPrefab : wirePrefab;
        GameObject gb = new GameObject($"seg {segIndex}");

        //GameObject gb = Instantiate(prefab, position, Quaternion.identity, segments);
        gb.name = $"seg {segIndex}";
        segIndex++;
        gb.transform.SetParent(segments);
        gb.transform.position = position;

        GameObject visual = Instantiate(prefab, gb.transform);

        visual.transform.localPosition = new Vector3(3.4f, 0, 0);

        var rb = gb.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.linearDamping = 2f;
        rb.angularDamping = 2f;

        var col = gb.AddComponent<SphereCollider>();
        col.radius = radius;
        col.excludeLayers = LayerMask.GetMask("Player");

        //configureable joint
        var joint=  gb.AddComponent<ConfigurableJoint>();
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        if (isPlug)
        {
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
        }

        SoftJointLimit limit = joint.linearLimit;
        limit.limit = spacing;
        joint.linearLimit = limit;

        return gb.transform;
        

    }

    private void FixedUpdate()
    {
        //segs[0].position = startTransform.position;

        TryAddSegment();

        var lastRB = segs[segs.Count - 1].GetComponent<Rigidbody>();
        //lastRB.isKinematic = false;
        lastRB.linearVelocity = Vector3.zero;
        if (isLockedToEndPoint)
        {
            lastRB.MovePosition(lockedPosition);
        }
        else
        {
            lastRB.MovePosition(endTransform.position);
        }

        Debug.Log("Current Len: " + CalcRealLength() + " Max Len: " + maxRouteLength);
            

    }


    private float CalcRouteLentgh()
    {
        float len = 0f;
        for(int i = 0; i < routeParent.childCount-1; i++)
        {
            len += Vector3.Distance(routeParent.GetChild(i).position, routeParent.GetChild(i + 1).position);
            
        }
        return len;
    }

    public float CalcRealLength()
    {
        float len = 0f;
        for(int i= 0; i<segs.Count-1; i++)
        {
            len += Vector3.Distance(segs[i].position, segs[i + 1].position);
        }
        return len;
    }
    void TryAddSegment()
    {
        if (CalcRealLength() >= maxRouteLength - 0.01f) return;
        if (isLockedToEndPoint) return;

        // last two sesgs
        Transform a = segs[0];
        Transform b = segs[segs.Count - 1];

        //Transform a = segs[segs.Count - 2].transform;
        //Transform b = segs[segs.Count - 1].transform;

        float disLst = Vector3.Distance(a.position, b.position);

        if (disLst <= (spacing*segs.Count) +0.01f) return;
        //if (disLst <= spacing + 0.02f) return;

        Vector3 dir = (a.position - b.position).normalized;
        Vector3 pos = b.position + dir * spacing;

        // new chain link
        Transform newSeg = CreateSeg(pos);

        //insert before plug
        int plugIndex = segs.Count - 1;
        segs.Insert(plugIndex, newSeg);

        // newSeg > prev
        newSeg.GetComponent<ConfigurableJoint>().connectedBody = segs[plugIndex - 1].GetComponent<Rigidbody>();

        // plug > newSeg
        var plug = segs[plugIndex + 1];
        var plugJoint = plug.GetComponent<ConfigurableJoint>();
        if(plugJoint == null)
        {
            plugJoint = plug.gameObject.AddComponent<ConfigurableJoint>();  
        }

        plugJoint.xMotion = ConfigurableJointMotion.Limited;
        plugJoint.yMotion = ConfigurableJointMotion.Limited;
        plugJoint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = plugJoint.linearLimit;
        limit.limit = spacing;
        plugJoint.linearLimit = limit;

        SoftJointLimitSpring spring = plugJoint.linearLimitSpring;
        spring.spring = 5000f;
        spring.damper = 0.2f;
        plugJoint.linearLimitSpring = spring;


        plugJoint.connectedBody = newSeg.GetComponent<Rigidbody>();

        //Destroy(segs[segs.Count-1].GetComponent<ConfigurableJoint>());

    }

    public float GetMaxLength() { return maxRouteLength; }

    public void LockTo(Vector3 pos)
    {
        isLockedToEndPoint = true;
        lockedPosition = pos;

        var lastRB = segs[segs.Count - 1].GetComponent<Rigidbody>();
        lastRB.isKinematic = true;
    }

    public void Unlock()
    {
        isLockedToEndPoint = false;
        var lastRB = segs[segs.Count - 1].GetComponent<Rigidbody>();
        lastRB.isKinematic = false;
    }
}
