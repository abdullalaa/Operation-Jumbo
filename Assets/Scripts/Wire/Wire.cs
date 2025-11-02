using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/*
Based reference from YouTube tutorial:
"Create Rope/Wire in Unity, Tutorial - From game idea to Steam - ep10"
by channel: Little Red Cabin
https://youtu.be/8rI1D1YQmhM?si=sxl-5a_LsNhwj4gN

This code is not a copy paste.
The original idea was sued as a base, but this version is modified for my own game logic
 */


public class Wire : MonoBehaviour
{
    [SerializeField] GameObject player;
    
    [Header("Anchor Settings")]
    // start point of wire
    [SerializeField] public Transform startTransform;
    // end point of wire
    [SerializeField] Transform endTransform;
    // number of rope segments
    // more segments = smoother rope
    [SerializeField] int segmentCount = 10;
    [SerializeField] public float totalLength = 40f;

    // thickness/size of each segment
    [SerializeField] float radius = 0.5f;

    [Header("Physics Settings")]
    // weight of the entire wire
    [SerializeField] float totalWeight = 10f;
    // lnear drag applied to each rigidbody segment
    [SerializeField] float drag = 1f;
    // angular drag applied to each rigidbody segment
    [SerializeField] float angularDrag = 1f;
    // if true
    // segments have collider & interact w world physics
    [SerializeField] bool usePhysics = false;

    [Header("Segment Settings")]
    // list of all segment Transforms created at runtime
    public Transform[] segments;
    // parent object where all segments are stored in hierarchy
    [SerializeField] Transform segmentParent;


    // used to detect inspector changes(rebuild rope)
    // previous value
    private int prevSegmentCount;
    // previous length
    private float prevTotalLength;
    // previous drag value
    private float prevDrag;
    // previous weight
    private float prevTotalWeight;
    // previous angular drag
    private float prevAngularDrag;
    // previous radius
    private float prevRadius;

    // line renderer used use to visually draw the rope path
    private LineRenderer lineRenderer;

        void Start()
    {
        // set default end position before game start
        if (!player.GetComponent<InteractionWEndPoint>().isConnected)
        {
            endTransform.position = player.transform.position + new Vector3(1f, 0, 0);
        }

        // initialize line renderer 
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        lineRenderer.widthMultiplier = radius * 0.5f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
    }



    // constantly rebuild / redraw wire
    // if inspector vaules changed during play mode
    void Update()
    {
        if (!player.GetComponent<InteractionWEndPoint>().isConnected)
        {
            endTransform.position = player.transform.position + new Vector3(0.5f, 0, 0);
        }

        if (prevSegmentCount != segmentCount)
        {
            RemoveSegments();
            segments = new Transform[segmentCount];
            GenerateSegments();
            
        }

        if (totalLength != prevTotalLength || prevDrag != drag || prevTotalWeight != totalWeight || prevAngularDrag != angularDrag)
        {
            UpdateWire();

        }

        prevSegmentCount = segmentCount;
        prevTotalLength = totalLength;
        prevDrag = drag;
        prevTotalWeight = totalWeight;
        prevAngularDrag = angularDrag;

        if (prevRadius != radius && usePhysics)
        {

            UpdateRadius();
            
        }

        prevRadius = radius;

        if (segments != null && segments.Length > 1 && lineRenderer != null)
        {
            var validSegs = segments.Where(s => s != null).ToList();
            lineRenderer.positionCount = validSegs.Count;
            for (int i = 0; i < validSegs.Count; i++)
            {
                lineRenderer.SetPosition(i, validSegs[i].position);
            }

        }

    }

    public void ResetWire()
    {
        RemoveSegments();
        segments = new Transform[segmentCount];
        GenerateSegments();

    }


    // create segment objects between start and end
    // and connect them with spring joints
    private void GenerateSegments()
    {
        JoinSegment(startTransform, null, true, true);
        Transform prevTransform = startTransform;

        Vector3 dir = (endTransform.position - startTransform.position);

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = new GameObject($"segment{i}");
            segment.transform.SetParent(segmentParent);
            segments[i] = segment.transform;

            Vector3 pos = (i == 0) ? prevTransform.position
                : prevTransform.position + (dir / segmentCount);

            segment.transform.position = pos;

            JoinSegment(segment.transform, prevTransform, false, i == 0);
            prevTransform = segment.transform;
        }
        JoinSegment(endTransform, prevTransform, true, true);

    }

    // attach rigidbody and spring joint to this segment
    // connect to previous one
    private void JoinSegment(Transform current, Transform connectedTrans, bool isKinetic = false, bool isCloseConnected = false)
    {
        if (current.GetComponent<Rigidbody>() == null)
        {
            Rigidbody rig = current.AddComponent<Rigidbody>();
            rig.mass = totalWeight / segmentCount;
            rig.linearDamping = drag;
            rig.angularDamping = angularDrag;
            rig.isKinematic = isKinetic;
        }

        if (usePhysics)
        {
            SphereCollider sphereCollider = current.AddComponent<SphereCollider>();
            sphereCollider.radius = radius;
            sphereCollider.isTrigger = false;
        }

        if (connectedTrans != null)
        {
            SpringJoint joint = current.GetComponent<SpringJoint>();
            if (joint == null)
            {
                joint = current.AddComponent<SpringJoint>();
            }
            joint.connectedBody = connectedTrans.GetComponent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = false;
            if (isCloseConnected)
            {
                joint.autoConfigureConnectedAnchor = true;
                joint.anchor = Vector3.zero;
            }
            else
            {
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = Vector3.forward * (totalLength / segmentCount);
            }
        }
    }

    // udate segment collider radius when changed in inspector
    private void UpdateRadius()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            SetRadiusOnSegment(segments[i], radius);
        }
    }


    private void SetRadiusOnSegment(Transform segment, float radius)
    {
        SphereCollider sphereCollider = transform.GetComponent<SphereCollider>();
        if (sphereCollider != null)
        {
            sphereCollider.radius = radius;
        }

    }

    // apply new length / weight / drag values to each rope segment
    private void UpdateWire()
    {
        for (int i = 0; i < segments.Length; i++)
        {

            if (i != 0)
            {
                UpdateLenghtOnSegment(segments[i]);
            }
            UpdateWeightOnSegment(segments[i], totalWeight, drag, angularDrag);
        }
    }

    // update joint anchor based on total rope length and segment count
    private void UpdateLenghtOnSegment(Transform transform)
    {
        ConfigurableJoint joint = transform.GetComponent<ConfigurableJoint>();
        if (joint != null)
        {
            joint.connectedAnchor = Vector3.forward * totalLength / segmentCount;
        }
    }

    // aply mass and drag settings to this segment rigidbody

    private void UpdateWeightOnSegment(Transform transform, float totalWeight, float drag, float angularDrag)
    {
        Rigidbody rb = transform.GetComponent<Rigidbody>();
        rb.mass = totalWeight / segmentCount;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
    }

    // destroy all old segments before creating new ones
    private void RemoveSegments()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] != null)
            {
                Destroy(segments[i].gameObject);
            }
        }
    }
    
    // return total length measured across every segment from start to end
    public float GetRealTotalLength()
    {
        if (segments == null || segments.Length == 0)
        {
            return 0f;
        }
        float length = 0f;

        Transform prev = startTransform;

        for (int i = 0; i < segments.Length; i++)
        {
            if (segments[i] != null)
            {
                length += Vector3.Distance(prev.position, segments[i].position);
                prev = segments[i];
            }
        }

        if (endTransform != null)
        {
            length += Vector3.Distance(prev.position, endTransform.position);
        }

        return length;
    }
}
