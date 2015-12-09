using UnityEngine;
using System.Collections;

public class Pipe : MonoBehaviour {

    public float pipeRadius;
    public int pipeSegmentCount;
    public float ringDistance;

    public float minCurveRadius, maxCurveRadius;
    public int minCurveSegmentCount, maxCurveSegmentCount;

    public PipeItemGenerator[] generators;

    private float curveRadius;
    private int curveSegmentCount;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private float curveAngle; // 
    private float relativeRotation; // random rotation around the x-axis
    private Vector2[] uv;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
    }

    public void Generate(bool withItems = true)
    {
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        curveSegmentCount =
            Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
        mesh.Clear();
        SetVertices();
        SetUV();
        SetTriangles();
        mesh.RecalculateNormals();

        // create mesh collider
        transform.gameObject.AddComponent<MeshCollider>();
        transform.GetComponent<MeshCollider>().sharedMesh = mesh;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        if (withItems)
        {
            generators[Random.Range(0, generators.Length)].GenerateItems(this);
        }
    }

    // using geometrical formula for a torus, find point on the torus in world space
    // u is angle in radians on curve
    // v is angle in radians on pipe cross-section circle
    private Vector3 GetPointOnTorus(float u, float v)
    {
        Vector3 p;
        float r = (curveRadius + pipeRadius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = pipeRadius * Mathf.Sin(v);
        return p;
    }

    private Vector3 GetPointAtCenterOfPipe(float u)
    {
        float originX = 0f;
        float originY = 0f;

        Vector3 point;
        point.x = originX + curveRadius * Mathf.Sin(u);
        point.y = originY + curveRadius * Mathf.Cos(u);
        point.z = 0f;

        return transform.TransformPoint(point);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(new Vector3(0f, 0f, 0f), 0.1f);
        float uStep = ringDistance / curveRadius;
        //    float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        //    for (int u = 0; u < curveSegmentCount; u++)
        //    {
        //        for (int v = 0; v < pipeSegmentCount; v++)
        //        {
        //            Vector3 point = GetPointOnTorus(u * uStep, v * vStep);
        //            Gizmos.color = new Color(
        //                1f,
        //                (float)v / pipeSegmentCount,
        //                (float)u / curveSegmentCount);
        //            Gizmos.DrawSphere(point, 0.1f);
        //        }
        //    }
        for (int u = 0; u < curveSegmentCount; u++)
        {
            Vector3 point = GetPointAtCenterOfPipe(u * uStep);
            Gizmos.color = new Color(
                        1f,
                        1f,
                        (float)u / curveSegmentCount);
            Gizmos.DrawSphere(point, 0.1f);
        }
    }

    private void SetVertices()
    {
        vertices = new Vector3[pipeSegmentCount * curveSegmentCount * 4];

        float uStep = ringDistance / curveRadius;
        curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));
        CreateFirstQuadRing(uStep);
        int iDelta = pipeSegmentCount * 4;
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i);
        }
        mesh.vertices = vertices;
    }

    private void CreateQuadRing(float u, int i)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;
        int ringOffset = pipeSegmentCount * 4;

        Vector3 vertex = GetPointOnTorus(u, 0f);
        for (int v = 1; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep);
        }
    }

    private void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f);
        Vector3 vertexB = GetPointOnTorus(u, 0f);
        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep);
        }
    }

    private void SetUV()
    {
        uv = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i += 4)
        {
            uv[i] = Vector2.zero;
            uv[i + 1] = Vector2.right;
            uv[i + 2] = Vector2.up;
            uv[i + 3] = Vector2.one;
        }
        mesh.uv = uv;
    }

    private void SetTriangles()
    {
        triangles = new int[pipeSegmentCount * curveSegmentCount * 6];
        for (int t = 0, i = 0; t < triangles.Length; t += 6, i += 4)
        {
            triangles[t] = i;
            triangles[t + 1] = triangles[t + 4] = i + 2;
            triangles[t + 2] = triangles[t + 3] = i + 1;
            triangles[t + 5] = i + 3;
        }
        mesh.triangles = triangles;
    }

    public void AlignWith(Pipe pipe)
    {
        relativeRotation = Random.Range(0, curveSegmentCount) * 360f / pipeSegmentCount;

        transform.SetParent(pipe.transform, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0f, 0f, -pipe.curveAngle);
        transform.Translate(0f, pipe.curveRadius, 0f);
        transform.Rotate(relativeRotation, 0f, 0f);
        transform.Translate(0f, -curveRadius, 0f);
        transform.SetParent(pipe.transform.parent);
        transform.localScale = Vector3.one; // prevent transform degradation when changing parents
    }

    public float CurveRadius
    {
        get
        {
            return curveRadius;
        }
    }

    public float CurveAngle
    {
        get
        {
            return curveAngle;
        }
    }

    public float RelativeRotation
    {
        get
        {
            return relativeRotation;
        }
    }

    public int CurveSegmentCount
    {
        get
        {
            return curveSegmentCount;
        }
    }

    //void Update()
    //{
    //    ringDistance -= 0.001f;
    //}
}
