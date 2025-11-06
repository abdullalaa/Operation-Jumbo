//using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditorInternal;
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
    [SerializeField] Material wireMeterial;

    [Header("Plug")]
    public bool isLockedToEndPoint = false;
    public Vector3 lockedPosition;

    List<Transform> segs = new List<Transform> ();
    float spacing;
    float maxRouteLength;
    int segIndex = 1;
    LineRenderer lr;


    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 0;
        lr.material = wireMeterial;

        lr.startWidth = radius * 0.5f;
        lr.endWidth = radius * 0.5f;
        lr.useWorldSpace = true;

        // allighment
        lr.alignment = LineAlignment.View;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // remove all seg
        for (int i = segments.childCount - 1; i >= 0; i--)
        {
            Destroy(segments.GetChild(i).gameObject);
        }

        segs.Clear();

        spacing = radius * 1.5f;
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
        GameObject gb = new GameObject($"seg {segIndex}");

        //GameObject gb = Instantiate(prefab, position, Quaternion.identity, segments);
        segIndex++;
        gb.transform.SetParent(segments);
        gb.transform.position = position;


   

        var rb = gb.AddComponent<Rigidbody>();
        rb.mass = 0.1f;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.1f;

        var col = gb.AddComponent<SphereCollider>();
        col.radius = radius;
        col.excludeLayers = LayerMask.GetMask("Player");

        //configureable joint
        var joint=  gb.AddComponent<ConfigurableJoint>();
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;

        SoftJointLimit limit = joint.linearLimit;
        limit.limit = spacing * 1.2f;
        joint.linearLimit = limit;

        if (isPlug)
        {
            joint.angularXMotion = ConfigurableJointMotion.Locked;
            joint.angularYMotion = ConfigurableJointMotion.Locked;
            joint.angularZMotion = ConfigurableJointMotion.Locked;
            rb.freezeRotation = true;
            //rb.isKinematic = true;
        }



        return gb.transform;
        

    }

    private void FixedUpdate()
    {
        //segs[0].position = startTransform.position;
        var firstRB = segs[0].GetComponent<Rigidbody>();
        firstRB.isKinematic = true;
        firstRB.MovePosition(startTransform.position);

        TryAddSegment();

        var lastRB = segs[segs.Count - 1].GetComponent<Rigidbody>();
        //lastRB.isKinematic = false;
        //lastRB.linearVelocity = Vector3.zero;
        if (isLockedToEndPoint)
        {
            lastRB.isKinematic = true; 
            lastRB.MovePosition(lockedPosition);
            //Vector3 dir=  (lockedPosition - lastRB.position);
            //lastRB.AddForce(dir.normalized * 2000f, ForceMode.Acceleration);
        }
        else
        {
            //lastRB.MovePosition(endTransform.position);
            lastRB.isKinematic= false;
            Vector3 dir = (endTransform.position - lastRB.position);
            lastRB.AddForce(dir.normalized * 10f, ForceMode.Acceleration);
        }

        

        Debug.Log("Current Len: " + CalcRealLength() + " Max Len: " + maxRouteLength);
            

    }

    private void LateUpdate()
    {
        int smoothAmount = 6;
        List<Vector3> pts = new List<Vector3>();

        lr.positionCount = (segs.Count-2)*smoothAmount+2;

        int idx = 0;

        lr.SetPosition(idx++, segs[0].position);

        for (int i = 1; i < segs.Count-1; i++)
        {

            Vector3 prev = segs[i - 1].position;
            Vector3 curr = segs[i].position;
            Vector3 next = segs[i+1].position;

            Vector3 center = (prev + curr + next) / 3f;

            for(int s = 1; s < smoothAmount; s++)
            {
                float t = s / (float)(smoothAmount + 1);
                Vector3 smoothedPos = Vector3.Lerp(curr, center, t);
                pts.Add(smoothedPos);
            }
            
        }

        pts.Add(segs[segs.Count - 1].position);
        lr.positionCount = pts.Count;
        lr.SetPositions(pts.ToArray());
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
        if (isLockedToEndPoint)
        {
            Debug.Log("Already Locked");
            return;
        }


        if (CalcRealLength() >= maxRouteLength - 0.01f)
        {
            Debug.Log("Over the Max");
            return;
        }

        // last two sesgs
        Transform a = segs[0];
        Transform b = segs[segs.Count - 1];

        //Transform a = segs[segs.Count - 2].transform;
        //Transform b = segs[segs.Count - 1].transform;

        float disLst = Vector3.Distance(a.position, b.position);
        Vector3 dir = (a.position - b.position).normalized;
        Vector3 pos = b.position + dir * spacing;

        if (disLst <= (spacing*segs.Count) +0.02f)
        {
            Debug.Log("Not Enough Space");
            return;
        }
        //if (disLst <= spacing + 0.02f) return;



        // new chain link
        Transform newSeg = CreateSeg(pos);
        Debug.Log("New seg Position: " + newSeg.position);

        //insert before plug
        int plugIndex = segs.Count - 1;
        segs.Insert(plugIndex, newSeg);

        // newSeg > prev seg connect
        newSeg.GetComponent<ConfigurableJoint>().connectedBody 
            = segs[plugIndex - 1].GetComponent<Rigidbody>();

        // plug > newSeg seg connect
        var plug = segs[plugIndex];
        var plugJoint = plug.GetComponent<ConfigurableJoint>();
        plugJoint.connectedBody = newSeg.GetComponent<Rigidbody>();

        SoftJointLimitSpring spring = plugJoint.linearLimitSpring;
        spring.spring = 2000f;
        spring.damper = 500f;
        plugJoint.linearLimitSpring = spring;


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
