/* 
 * Create mesh for FOV, so it can be seen during game.
 * Added updateColor
 */

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfViewMesh : MonoBehaviour
{
    public int meshResolution = 50; // How many rays are cast across FOV (higher > smoother)
    public int edgeResolveIterations = 4; // How many iterations used to refine edges of mesh, where obstruction is
    public float edgeDstThreshold = 0.5f; // Minimum difference between rays to consider new edge (for smaller obstacles)

    private FieldOfView fov;
    private Mesh viewMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Color currentColor;

    void Start()
    {
        fov = GetComponentInParent<FieldOfView>();

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        viewMesh = new Mesh();
        viewMesh.name = "FOV Mesh";
        meshFilter.mesh = viewMesh;

        currentColor = fov.baseColor;
    }

    void Update()
    {
        DrawFOVMesh();
        UpdateColor(fov.playerInFOV);
    }

    // Transistion mesh colors based on whether player is detected
    void UpdateColor(bool playerSeen)
    {
        Color targetColor;

        if (playerSeen)
        {
            targetColor = fov.alertColor;
        }
        else
        {
            targetColor = fov.baseColor;
        }

        // Transistion over time
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * fov.detectionTime);
        meshRenderer.material.color = currentColor;
    }

    // Generate FOV Mesh with edge refinement instead of raycast
    void DrawFOVMesh()
    {
        // Calculate how many rays to cast based on angle and mesh resolution
        int rayCount = Mathf.RoundToInt(fov.angle * meshResolution / 360f);
        float rayAngleSize = fov.angle / rayCount;

        // All points where rays hit or reach max distance
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo oldViewCast = new ViewCastInfo();

        // Cast rays across FOV
        for (int i = 0; i <= rayCount; i++)
        {
            float angle = transform.eulerAngles.y - fov.angle / 2 + rayAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // Edge refinement: if previous ray hit is different from current, refine edge
            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero) viewPoints.Add(edge.pointA);
                    if (edge.pointB != Vector3.zero) viewPoints.Add(edge.pointB);
                }
            }

            viewPoints.Add(newViewCast.point);
            oldViewCast = newViewCast;
        }

        // Calculate vertices and triangles for mesh
        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        // Center of the mesh is at the empty
        vertices[0] = Vector3.zero;

        // Convert world-space points into local space relative to FOV origin
        for (int i = 0; i < viewPoints.Count; i++)
        {
            Vector3 localPos = fov.transform.InverseTransformPoint(viewPoints[i]); // By using inversetransformpoint
            vertices[i + 1] = new Vector3(localPos.x, 0f, localPos.z);
        }

        // Convert vertices to form triangles
        for (int i = 0; i < vertexCount - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Update mesh with new vertices and triangles
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals(); // Correct lighting
    }

    // Cast ray at angle to detect obstruction
    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        // If ray hit obstacle return that point
        if (Physics.Raycast(fov.transform.position, dir, out hit, fov.radius, fov.obstructionLayer))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        // If ray does not hit return point at max radius
        else
        {
            return new ViewCastInfo(false, fov.transform.position + dir * fov.radius, fov.radius, globalAngle);
        }
    }

    // Refine edge points between 2 rays
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        // Cast new ray to narrow down exact edge point
        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            // Adjust angle range based on hit result
            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new EdgeInfo(minPoint, maxPoint);
    }

    // Convert angle to 3D direction vector
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal) angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    // Struct to store raycast hit
    public struct ViewCastInfo
    {
        public bool hit; // Did ray hit obstruction
        public Vector3 point; // Position of hit or max radius
        public float dst; // Distance from FOV origin to point
        public float angle; // Angle at which ray was cast

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

    // Struct to store 2 points of an edge between rays
    public struct EdgeInfo
    {
        public Vector3 pointA;
        public Vector3 pointB;

        public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
        {
            pointA = _pointA;
            pointB = _pointB;
        }
    }
}
