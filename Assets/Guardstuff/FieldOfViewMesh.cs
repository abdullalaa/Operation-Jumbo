/* 
 * Create mesh for FOV, so it can be seen during game.
 * Added updateColor and transparency??
 */

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class FieldOfViewMesh : MonoBehaviour
{
    public int meshResolution = 50;
    public int edgeResolveIterations = 4;
    public float edgeDstThreshold = 0.5f;

    private FieldOfView fov;
    private Mesh viewMesh;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    private Color currentColor;

    void Awake()
    {
        fov = GetComponentInParent<FieldOfView>();
        if (fov == null)
            Debug.LogError("No FieldOfView component found in parent objects.");

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        viewMesh = new Mesh();
        viewMesh.name = "FOV Mesh";
        meshFilter.mesh = viewMesh;

        currentColor = fov.baseColor;
    }

    void LateUpdate()
    {
        if (fov == null) return;

        DrawFOVMesh();
        UpdateColor(fov.playerInFOV);
    }

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

        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * fov.colorTransitionSpeed);
        meshRenderer.material.color = currentColor;
    }

    void DrawFOVMesh()
    {
        int stepCount = Mathf.RoundToInt(fov.angle * meshResolution / 360f);
        float stepAngleSize = fov.angle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();

        ViewCastInfo oldViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = transform.eulerAngles.y - fov.angle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

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

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];

        // center of the mesh is at the empty
        vertices[0] = Vector3.zero;

        for (int i = 0; i < viewPoints.Count; i++)
        {
            // flatten to the empty's Y level
            Vector3 localPos = fov.transform.InverseTransformPoint(viewPoints[i]);
            //Vector3 localPos = viewPoints[i] - transform.position;
            vertices[i + 1] = new Vector3(localPos.x, 0f, localPos.z);
        }

        for (int i = 0; i < vertexCount - 2; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(fov.transform.position, dir, out hit, fov.radius, fov.obstructionLayer))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
        }
        else
        {
            return new ViewCastInfo(false, fov.transform.position + dir * fov.radius, fov.radius, globalAngle);
        }
    }

    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

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

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal) angleInDegrees += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
        }
    }

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
