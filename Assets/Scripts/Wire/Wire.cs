using NUnit.Framework.Constraints;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class Wire : MonoBehaviour
{
    [Header("Anchor Settings")]
    [SerializeField] public Transform startTransform;
    [SerializeField] Transform endTransform;
    [SerializeField] int segmentCount = 10;
    [SerializeField] public float totalLength = 10f;

    [SerializeField] float radius = 0.5f;
    [SerializeField] int sides = 4;

    [Header("Physics Settings")]
    [SerializeField] float totalWeight = 10f;
    [SerializeField] float drag = 1f;
    [SerializeField] float angularDrag = 1f;

    [SerializeField] bool usePhysics = false;

    [Header("Segment Settings")]
    public Transform[] segments;
    [SerializeField] Transform segmentParent;

    private int prevSegmentCount;
    private float prevTotalLength;
    private float prevDrag;
    private float prevTotalWeight;
    private float prevAngularDrag;
    private float prevRadius;

    private Vector3[] vertices;
    private int[,] vertexIndicesMap;

    private LineRenderer lineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        vertices = new Vector3[segmentCount * sides * 3];
        GenerateMesh();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        lineRenderer.widthMultiplier = radius * 0.5f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.blue;
        lineRenderer.endColor = Color.blue;
    }

    void Update()
    {
        if (prevSegmentCount != segmentCount)
        {
            RemoveSegments();
            segments = new Transform[segmentCount];
            GenerateSegments();
            GenerateMesh();
        }

        if (totalLength != prevTotalLength || prevDrag != drag || prevTotalWeight != totalWeight || prevAngularDrag != angularDrag)
        {
            UpdateWire();
            GenerateMesh();

        }

        prevSegmentCount = segmentCount;
        prevTotalLength = totalLength;
        prevDrag = drag;
        prevTotalWeight = totalWeight;
        prevAngularDrag = angularDrag;

        if (prevRadius != radius && usePhysics)
        {

            UpdateRadius();
            GenerateMesh();
        }

        prevRadius = radius;

        if (segments != null && segments.Length > 1 && lineRenderer != null) {
            var validSegs = segments.Where(s => s != null).ToList();
            lineRenderer.positionCount = validSegs.Count;
            for (int i = 0; i < validSegs.Count; i++)
            {
                lineRenderer.SetPosition(i, validSegs[i].position);
            }

        }

    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            Gizmos.DrawWireSphere(segments[i].position, radius);
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }

    private void GenerateSegments()
    {
        JoinSegment(startTransform, null, true);
        Transform prevTransform = startTransform;

        Vector3 dir = (endTransform.position - startTransform.position);

        for (int i = 0; i < segmentCount; i++)
        {
            GameObject segment = new GameObject($"segment{i}");
            segment.transform.SetParent(segmentParent);
            segments[i] = segment.transform;

            Vector3 pos = prevTransform.position + (dir / segmentCount);
            segment.transform.position = pos;

            JoinSegment(segment.transform, prevTransform);
            prevTransform = segment.transform;
        }
        JoinSegment(endTransform, prevTransform, true, true);

    }

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
            sphereCollider.isTrigger = true;
        }

        if (connectedTrans != null) {
            ConfigurableJoint joint = current.GetComponent<ConfigurableJoint>();
            if (joint == null)
            {
                joint = current.AddComponent<ConfigurableJoint>();
            }
            joint.connectedBody = connectedTrans.GetComponent<Rigidbody>();
            joint.autoConfigureConnectedAnchor = false;
            if (isCloseConnected)
            {
                joint.connectedAnchor = Vector3.forward * 0.1f;
            }
            else
            {
                joint.connectedAnchor = Vector3.forward * (totalLength / segmentCount);
            }

            joint.xMotion = ConfigurableJointMotion.Locked;
            joint.yMotion = ConfigurableJointMotion.Locked;
            joint.zMotion = ConfigurableJointMotion.Locked;

            joint.angularXMotion = ConfigurableJointMotion.Free;
            joint.angularYMotion = ConfigurableJointMotion.Free;
            joint.angularZMotion = ConfigurableJointMotion.Limited;

            SoftJointLimit softJointLimit = new SoftJointLimit();
            softJointLimit.limit = 0;
            joint.angularZLimit = softJointLimit;

            JointDrive jointDrive = new JointDrive();
            jointDrive.positionDamper = 0;
            jointDrive.positionSpring = 0;
            joint.angularXDrive = jointDrive;
            joint.angularYZDrive = jointDrive;


        }
    }

    // Update is called once per frame


    private void UpdateRadius()
    {
        for (int i = 0; i < segments.Length; i++)
        {
            SetRadiusOnSegment(segments[i], radius);
        }
    }

    private void GenerateVertices()
    {

        for (int i = 0; i < segments.Length; i++)
        {
            GenerateCircleVerticles(segments[i], i);
        }
    }

    private void GenerateCircleVerticles(Transform segTrans, int segmentIndex)
    {
        float angleDiff = 360 / sides;

        Quaternion diffRotation = Quaternion.FromToRotation(Vector3.forward, segTrans.forward);
        for (int sideIndex = 0; sideIndex < sides; sideIndex++)
        {
            float angleInRad = sideIndex * angleDiff * Mathf.Deg2Rad;
            float x = -1 * radius * Mathf.Cos(angleInRad);
            float y = radius * Mathf.Sin(angleInRad);

            Vector3 pointOffset = new(x, y, 0);
            Vector3 pointRotated = diffRotation * pointOffset;

            // vertical position
            Vector3 pointRotatedAtCenterOfTransform = segTrans.position + pointRotated;

            int vertexIndex = segmentIndex * sides + sideIndex;
            vertices[vertexIndex] = pointRotatedAtCenterOfTransform;

        }
    }
    void updateMMesh()
    {
        GenerateVertices();
    }

    void GenerateMesh()
    {
        GenerateIndicesMap();
        GenerateVertices();

    }

    private void GenerateIndicesMap()
    {
        vertexIndicesMap = new int[segmentCount + 1, sides + 1];
        int meshVertexIndex = 0;
        for (int segIndex = 0; segIndex < segmentCount; segIndex++)
        {
            for (int sideIndex = 0; sideIndex < sides; sideIndex++)
            {
                vertexIndicesMap[segIndex, sideIndex] = meshVertexIndex;
                meshVertexIndex++;
            }
        }
    }

    private void SetRadiusOnSegment(Transform segment, float radius)
    {
        SphereCollider sphereCollider = transform.GetComponent<SphereCollider>();
        if (sphereCollider != null) {
            sphereCollider.radius = radius;
        }

    }

    private void UpdateWire()
    {
        for (int i = 0; i < segments.Length; i++)
        {

            if (i != 0)
            {
                UpdateLenghtOnSegment(segments[i], totalLength / segmentCount);
            }
            UpdateWeightOnSegment(segments[i], totalWeight, drag, angularDrag);
        }
    }

    private void UpdateLenghtOnSegment(Transform transform, float y)
    {
        ConfigurableJoint joint = transform.GetComponent<ConfigurableJoint>();
        if (joint != null)
        {
            joint.connectedAnchor = Vector3.forward * totalLength / segmentCount;
        }
    }

    private void UpdateWeightOnSegment(Transform transform, float totalWeight, float drag, float angularDrag)
    {
        Rigidbody rb = transform.GetComponent<Rigidbody>();
        rb.mass = totalWeight / segmentCount;
        rb.linearDamping = drag;
        rb.angularDamping = angularDrag;
    }

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

    public float GetCurrentLength()
    {
        if (startTransform == null || endTransform == null)
        {
            return 0f;
        }

        return Vector3.Distance(startTransform.position, endTransform.position);
    }
}
