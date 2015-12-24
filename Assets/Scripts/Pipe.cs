using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pipe : MonoBehaviour {

    public float pipeRadius;
    public int pipeSegmentCount;
    public float ringDistance; // scaling of pipe rings; default is 1

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
    
    public BezierSpline cameraSpline;
    private List<Vector3> centerPoints;
    private MeshCollider meshCollider;

    //三点一平面
    private Vector3 p0 = Vector3.zero;
    private Vector3 p1 = Vector3.zero;
    private Vector3 worldPositionOfAvatar = new Vector3(111,222,333);

    private float angleOfPiple;
    // private float

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
        
        // create camera bezier spline
        cameraSpline = gameObject.AddComponent<BezierSpline>();
        centerPoints = new List<Vector3>();

        // get mesh collider
        meshCollider = GetComponent<MeshCollider>();
    }

    public void Generate(bool withItems = true)
    {
        // create pipe mesh
        curveRadius = Random.Range(minCurveRadius, maxCurveRadius);
        curveSegmentCount = Random.Range(minCurveSegmentCount, maxCurveSegmentCount + 1);
        mesh.Clear();
        SetVertices();
        SetUV();
        SetTriangles();
        mesh.RecalculateNormals();

        // set mesh collider to use newly created mesh
        meshCollider.sharedMesh = mesh;

        // generate camera spline
        cameraSpline.Init(GetCenterPoints().ToArray());

        // clean up obstacles for recycled pipe
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        // create new obstacles for this pipe
        if (withItems)
        {
            generators[Random.Range(0, generators.Length)].GenerateItems(this);
        }

        float u = curveSegmentCount * ringDistance / curveRadius;
        p0.x = curveRadius * Mathf.Sin(0);
        p0.y = curveRadius * Mathf.Cos(0);
        p0.z = 0;

        p1.x = curveRadius * Mathf.Sin(u);
        p1.y = curveRadius * Mathf.Cos(u);
        p1.z = 0;
        angleOfPiple = Vector3.Angle(p0,p1);
    }

    // using geometrical formula for a torus, find point on the torus in world space
    // u is angle in radians along curve
    // v is angle in radians along pipe cross-section circle
    private Vector3 GetPointOnTorus(float u, float v)
    {
        Vector3 p;
        float r = (curveRadius + pipeRadius * Mathf.Cos(v));
        p.x = r * Mathf.Sin(u);
        p.y = r * Mathf.Cos(u);
        p.z = pipeRadius * Mathf.Sin(v);
        return p;
    }

    public void GetPlaneOfCurve(Vector3 worldPositionOfAvatar, ref Vector3 directionOutput,ref Vector3 pointOutput,ref float progress)
    {
        Vector3 p0world;
        Vector3 p1world;
        this.worldPositionOfAvatar = transform.InverseTransformPoint(worldPositionOfAvatar);

        p0world = transform.TransformPoint(p0);
        p1world = transform.TransformPoint(p1);

        Vector3 normal = new Vector3();
        Vector3 point = new Vector3();
        //计算圆弧所在的平面
        Math3d.PlaneFrom3Points(out normal,out point, p0, p1,Vector3.zero);

        //计算车的位置在平面上的投影点
        Vector3 projectionPoint = Math3d.ProjectPointOnPlane(normal, point, worldPositionOfAvatar);
        Vector3 pointCenter = projectionPoint.normalized * p0.magnitude;

        //转换为世界坐标输出
        pointOutput = transform.TransformPoint(pointCenter);
        Vector3 pointCenterDirection = new Vector3(
                1,
                -pointCenter.x/pointCenter.y,
                0).normalized;
        directionOutput = transform.TransformVector(pointCenterDirection).normalized;
        //计算progress
        //圆弧角度不大于270度即可
        float angle = angle_360(p0, pointCenter);
        if(angle >= 270)
        {
            progress = (270-360)/angleOfPiple;
        }else{
            progress = angle/angleOfPiple;
        }

    }

    //计算夹角的角度 0~360
    private static float angle_360(Vector3 from_, Vector3 to_){

        Vector3 v3 = Vector3.Cross(from_,to_);

        if(v3.z > 0)

            return Vector3.Angle(from_,to_);

        else

            return 360-Vector3.Angle(from_,to_);

    }

    private void OnDrawGizmos()
    {
        Vector3 p0world;
        Vector3 p1world;

        //计算圆弧所在的平面
        p0world = transform.TransformPoint(p0);
        p1world = transform.TransformPoint(p1);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p0world, 0.5f);
        Gizmos.DrawSphere(p1world, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.TransformPoint(Vector3.zero), 0.5f);

        //计算车的位置在平面上的投影点
        Vector3 normal = new Vector3();
        Vector3 point = new Vector3();
        Math3d.PlaneFrom3Points(out normal,out point, p0, p1,Vector3.zero);

        Vector3 projectionPoint = Math3d.ProjectPointOnPlane(normal, point, worldPositionOfAvatar);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.TransformPoint(projectionPoint), 0.5f);

        //get center
        Vector3 pointCenter = projectionPoint.normalized * p0.magnitude;
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.TransformPoint(pointCenter), 0.3f);

        Vector3 pointCenterDirection = new Vector3(
                1,
                -pointCenter.x/pointCenter.y,
                0);
        //圆切线
        Gizmos.DrawLine(transform.TransformPoint(pointCenter),
                transform.TransformPoint(pointCenter)+transform.TransformVector(pointCenterDirection).normalized*5f);
    }

    // using geometrical formula for a circle, find point along curve
    // in the center of the pipe, in world space
    // u is angle in radians along curve
    private Vector3 GetPointAtCenterOfPipe(float u)
    {
        float originX = 0f;
        float originY = 0f;

        Vector3 point;
        point.x = originX + curveRadius * Mathf.Sin(u);
        point.y = originY + curveRadius * Mathf.Cos(u);
        point.z = 0f;

        //return transform.TransformPoint(point);
        return point;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawSphere(new Vector3(0f, 0f, 0f), 0.1f);
    //    float uStep = ringDistance / curveRadius;
    //    //    float vStep = (2f * Mathf.PI) / pipeSegmentCount;

    //    //    for (int u = 0; u < curveSegmentCount; u++)
    //    //    {
    //    //        for (int v = 0; v < pipeSegmentCount; v++)
    //    //        {
    //    //            Vector3 point = GetPointOnTorus(u * uStep, v * vStep);
    //    //            Gizmos.color = new Color(
    //    //                1f,
    //    //                (float)v / pipeSegmentCount,
    //    //                (float)u / curveSegmentCount);
    //    //            Gizmos.DrawSphere(point, 0.1f);
    //    //        }
    //    //    }
    //    for (int u = 0; u < curveSegmentCount; u++)
    //    {
    //        Vector3 point = GetPointAtCenterOfPipe(u * uStep);
    //        Gizmos.color = new Color(
    //                    1f,
    //                    1f,
    //                    (float)u / curveSegmentCount);
    //        Gizmos.DrawSphere(point, 0.1f);
    //    }
    //}

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
        // random relative rotation
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

    // get a list of the pipe's center points for the camera track
    private List<Vector3> GetCenterPoints()
    {
        centerPoints.Clear();
        float uStep = ringDistance / curveRadius;
        for (int u = 0; u < curveSegmentCount; u++)
        {
            Vector3 point = GetPointAtCenterOfPipe(u * uStep);
            centerPoints.Add(point);
        }

        return centerPoints;
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
}
