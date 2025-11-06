//using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;

//using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class PlugWire : MonoBehaviour
{

    [Header("Reference")]
    [SerializeField] public Transform startTransform;
    //[SerializeField] public Transform endTransform;
    [SerializeField] public Transform attachedPoint;
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

    [Header("For mesh")]
    MeshFilter mf;
    [SerializeField] int radialSegments = 10;
    [SerializeField] float tubeRadius = 0.1f;

    List<Transform> segs = new List<Transform>();
    float spacing;
    float maxRouteLength;
    int segIndex = 1;
    LineRenderer lr;


    private void Awake()
    {
        //lr = GetComponent<LineRenderer>();
        //lr.positionCount = 0;
        //lr.material = wireMeterial;

        //lr.startWidth = radius * 0.5f;
        //lr.endWidth = radius * 0.5f;
        //lr.useWorldSpace = true;

        //// allighment
        //lr.alignment = LineAlignment.View;

        mf = GetComponent<MeshFilter>();
        if(mf == null) mf = gameObject.AddComponent<MeshFilter>();

        var mr = GetComponent<MeshRenderer>();
        if(mr == null) mr = gameObject.AddComponent<MeshRenderer>();
        mr.material = wireMeterial;

        mf.mesh = new Mesh();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BuildWire();
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

            lastRB.isKinematic = true;
            lastRB.MovePosition(attachedPoint.position);
            //    Vector3 dir = (attachedPoint.position - lastRB.position);
            //lastRB.AddForce(dir.normalized * 2000f, ForceMode.Acceleration);
        }



        Debug.Log("Current Len: " + CalcRealLength() + " Max Len: " + maxRouteLength);


    }

    private void LateUpdate()
    {

        //lr.positionCount = segs.Count;
        //for (int i = 0; i < segs.Count; i++)
        //{
        //    lr.SetPosition(i, segs[i].position);
        //}

        List<Vector3> pts = new List<Vector3>();
        foreach(var s in segs)
        {
            pts.Add(s.position);
        }

        mf.mesh = CrerateTubeMesh(pts, tubeRadius, radialSegments);
        transform.position = Vector3.zero;

    }

    private Mesh CrerateTubeMesh(List<Vector3> points, float r, int rs)
    {
        Mesh mesh = new Mesh();

        int ringCount = points.Count;
        int vertCount = ringCount * rs;

        Vector3[] verts = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        int[] tris = new int[(ringCount - 1) * rs * 6];

        for(int i= 0; i< ringCount; i++)
        {
            Vector3 center = points[i];

            Vector3 forward = (i == ringCount - 1) ?
                (center - points[i - 1]).normalized :
                (points[i+1]-center).normalized;

            Vector3 up = Vector3.Cross(forward, Vector3.up);
            if (up == Vector3.zero) up = Vector3.Cross(forward, Vector3.right);

            for(int j= 0; j< rs; j++)
            {
                float angle = (float)j / rs * Mathf.PI * 2f;
                Quaternion rot = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, forward);
                Vector3 normal = (rot * up).normalized;

                int index = i * rs + j;
                verts[index] = center+ normal*r;
                normals[index] = normal;
            }
        }

        int t = 0;
        for(int i = 0; i<ringCount-1; i++)
        {
            for(int j = 0; j< rs; j++)
            {
                int current = i * rs + j;
                int next = current + rs;
                int currentPlusOne = i * rs + (j + 1) % rs;
                int nextPlusOne = currentPlusOne + rs;

                tris[t++] = current;
                tris[t++] = currentPlusOne;
                tris[t++] = nextPlusOne;

                tris[t++] = current;
                tris[t++] = nextPlusOne;
                tris[t++] = next;
            }
        }

        mesh.vertices = verts;
        mesh.normals = normals;
        mesh.triangles = tris;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        return mesh;
    }

    private void Update()
    {
        if (isLockedToEndPoint) return;
        attachedPoint.position = attachedPoint.position;
        segs[segs.Count - 1].GetComponent<SpringJoint>().connectedAnchor = attachedPoint.position;
    }


    Transform CreateSeg(Vector3 position, bool isPlug = false)
    {
        GameObject gb = new GameObject($"seg {segIndex}");

        //GameObject gb = Instantiate(prefab, position, Quaternion.identity, segments);
        segIndex++;
        gb.transform.SetParent(segments);
        gb.transform.position = position;




        var rb = gb.AddComponent<Rigidbody>();
        rb.mass = 0.02f;
        rb.linearDamping = 0.1f;
        rb.angularDamping = 0.1f;

        var col = gb.AddComponent<SphereCollider>();
        col.radius = radius;
        col.excludeLayers = LayerMask.GetMask("Player");
        //col.isTrigger = true;



        //configureable joint
        //var joint = gb.AddComponent<ConfigurableJoint>();
        //joint.xMotion = ConfigurableJointMotion.Limited;
        //joint.yMotion = ConfigurableJointMotion.Limited;
        //joint.zMotion = ConfigurableJointMotion.Limited;

        //SoftJointLimit limit = joint.linearLimit;
        //limit.limit = spacing * 2;
        //joint.linearLimit = limit;

        SpringJoint joint = gb.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.minDistance = 0f;
        joint.maxDistance = spacing * 0.2f;
        joint.spring = 500f;
        joint.damper = 500f;

        if (isPlug)
        {
            //    joint.angularXMotion = ConfigurableJointMotion.Locked;
            //    joint.angularYMotion = ConfigurableJointMotion.Locked;
            //    joint.angularZMotion = ConfigurableJointMotion.Locked;
            rb.freezeRotation = true;
            //rb.isKinematic = true;
        }
        if (!isPlug)
        {
            //col.isTrigger = true;
        }



        return gb.transform;


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

        //// before plug
        //Transform prev = segs[segs.Count - 2].transform;
        //Transform plug = segs[segs.Count - 1].transform;

        //float disLst = Vector3.Distance(prev.position, plug.position);
        float disLst = Vector3.Distance(a.position, b.position);
        Vector3 dir = (a.position - b.position).normalized;
        Vector3 pos = b.position + dir * spacing;

        if (disLst <= (spacing * segs.Count) + 0.02f)
        {
            Debug.Log("Not Enough Space");
            return;
        }
        //if (disLst < spacing *1.1f) return;
        //Vector3 dir = (plug.position - prev.position).normalized;
        //Vector3 pos = prev.position + dir * spacing;

        SpawnSegmentAt(pos);




        //Destroy(segs[segs.Count-1].GetComponent<ConfigurableJoint>());

    }

    private void SpawnSegmentAt(Vector3 pos)
    {
        // new chain link
        Transform plug = segs[segs.Count - 1];
        Transform newSeg = CreateSeg(pos);
        Debug.Log("New seg Position: " + newSeg.position);

        //insert before plug
        int plugIndex = segs.Count - 1;
        segs.Insert(plugIndex, newSeg);

        // newSeg > prev seg connect

        SpringJoint spring1 = newSeg.GetComponent<SpringJoint>();
        spring1.connectedBody = segs[plugIndex - 1].GetComponent<Rigidbody>();

        float d1 = Vector3.Distance(newSeg.position, segs[plugIndex - 1].position);
        spring1.minDistance = 0f;
        spring1.maxDistance = 0.1f;
        spring1.spring = 500f;
        spring1.damper = 500f;


        // plug > newSeg seg connect
        var plugJoint = plug.GetComponent<SpringJoint>();
        plugJoint.connectedBody = newSeg.GetComponent<Rigidbody>();

        float d2 = Vector3.Distance(segs[plugIndex].position, newSeg.position);
        plugJoint.minDistance = 0f;
        plugJoint.maxDistance = 0.1f;
        plugJoint.spring = 500f;
        plugJoint.damper = 500f;

        //SoftJointLimitSpring spring = plugJoint.linearLimitSpring;
        //spring.spring = 2000f;
        //spring.damper = 500f;
        //plugJoint.linearLimitSpring = spring;
    }

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

    private float CalcRouteLentgh()
    {
        float len = 0f;
        for (int i = 0; i < routeParent.childCount - 1; i++)
        {
            len += Vector3.Distance(routeParent.GetChild(i).position, routeParent.GetChild(i + 1).position);

        }
        return len;
    }

    public float CalcRealLength()
    {
        float len = 0f;
        for (int i = 0; i < segs.Count - 1; i++)
        {
            len += Vector3.Distance(segs[i].position, segs[i + 1].position);
        }
        return len;
    }

    public float GetMaxLength()
    {
        return CalcRouteLentgh();
    }

    public void ResetWire()
    {
        isLockedToEndPoint = false;
        lockedPosition = Vector3.zero;

        BuildWire();
    }
    void BuildWire()
    {
        // remove all seg
        for (int i = segments.childCount - 1; i >= 0; i--)
        {
            Destroy(segments.GetChild(i).gameObject);
        }

        segs.Clear();

        //spacing = radius * 1.5f;
        maxRouteLength = CalcRouteLentgh();
        spacing = maxRouteLength / 120f;

        // create first fixed segment == start
        var first = CreateSeg(startTransform.position);
        segs.Add(first);
        first.GetComponent<Rigidbody>().isKinematic = true;
        //segs[0].position = startTransform.position;

        // create 3 initial spheres
        for (int i = 1; i < initialSeg - 1; i++)
        {
            segs.Add(CreateSeg(startTransform.position));

        }
        segs.Add(CreateSeg(startTransform.position, true));

        //initial spread
        Vector3 dir = (player.transform.position - startTransform.position).normalized;

        for (int i = 1; i < segs.Count; i++)
        {
            segs[i].position = startTransform.position + dir * spacing * i;
        }

        for (int i = 1; i < segs.Count - 1; i++)
        {
            //segs[i].GetComponent<ConfigurableJoint>().connectedBody = segs[i-1].GetComponent<Rigidbody>();
            var spring = segs[i].GetComponent<SpringJoint>();
            spring.connectedBody = segs[i - 1].GetComponent<Rigidbody>();

            float d = Vector3.Distance(segs[i].position, segs[i - 1].position);
            spring.minDistance = 0f;
            spring.maxDistance = 0.1f;
            spring.spring = 500f;
            spring.damper = 10f;

        }

        var last = segs[segs.Count - 1];
        SpringJoint plugJoint = last.GetComponent<SpringJoint>();
        //plugJoint.connectedAnchor = endTransform.position;

        Rigidbody endRB = attachedPoint.GetComponent<Rigidbody>();
        plugJoint.connectedBody = endRB;
        plugJoint.autoConfigureConnectedAnchor = true;

        plugJoint.minDistance = 0f;
        plugJoint.maxDistance = 0.1f;
        plugJoint.spring = 200f;
        plugJoint.damper = 5000f;
        //Destroy(segs[segs.Count - 1].GetComponent<ConfigurableJoint>());
    }

}

