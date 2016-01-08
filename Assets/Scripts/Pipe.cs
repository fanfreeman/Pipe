using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pipe : MonoBehaviour {

    public float maxPipeRadius;
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
    private float curveAngle; // the entire curve span of the pipe in degrees
    private float relativeRotation; // random rotation around the x-axis
    private Vector2[] uv;
    
    public BezierSpline cameraSpline;
    private List<Vector3> centerPoints;
    private MeshCollider meshCollider;

    //三点一平面
    private Vector3 p0 = Vector3.zero;
    private Vector3 p1 = Vector3.zero;
    private Vector3 positionOfAvatar = new Vector3(111,222,333);

    private float angleOfPiple;

    public float pipeRadiusBegin; // 管道开头大小
    public float pipeRadiusEnd; // 管道结尾大小

    private MeshRenderer renderer;
    private Color pipeEmissionColor;

    public Pipe nextPipe
    {
        get; set;
    }

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Pipe";
        
        // create camera bezier spline
        cameraSpline = gameObject.AddComponent<BezierSpline>();
        centerPoints = new List<Vector3>();

        // get mesh collider
        meshCollider = GetComponent<MeshCollider>();

        renderer =  GetComponent<MeshRenderer>();
        pipeEmissionColor = renderer.material.GetColor("_EmissionColor");
    }

    public float GetPipeEndRadius()
    {
        return pipeRadiusEnd;
    }

    // prevPipeEndRadius is the radius of the end of the previous pipe
    public void Generate (float prevPipeEndRadius, bool withItems = true)
    {
        // determine pipe beginning and end radii
        pipeRadiusBegin = prevPipeEndRadius;
        pipeRadiusEnd = Random.Range(maxPipeRadius, maxPipeRadius);

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
        //generators[Random.Range(0, generators.Length)].GenerateItems(this);

        //扇形的2个顶点
        float u = curveSegmentCount * ringDistance / curveRadius;
        p0.x = curveRadius * Mathf.Sin(0);
        p0.y = curveRadius * Mathf.Cos(0);
        p0.z = 0;

        p1.x = curveRadius * Mathf.Sin(u);
        p1.y = curveRadius * Mathf.Cos(u);
        p1.z = 0;
        angleOfPiple = Vector3.Angle(p1,p0);
    }

    //粗细不同产生的的angle
    public float GetPipeAngle()
    {
        return -Mathf.Atan(
                Mathf.Abs((pipeRadiusBegin - pipeRadiusEnd))/
                (
                    curveRadius*2*Mathf.PI *
                        angleOfPiple/360
                )
        );
    }

    // using geometrical formula for a torus, find point on the torus in world space
    // u is angle in radians along curve
    // v is angle in radians along pipe cross-section circle
    // uNum is the step number along pipe curve
    private Vector3 GetPointOnTorus(float u, float v, int uNum)
    {
        Vector3 p;
        int inverseStepsToBeginning = CurveSegmentCount - uNum;
        int inverseStepsToEnd = uNum;
        float pipeRadius = (inverseStepsToBeginning * pipeRadiusBegin + inverseStepsToEnd * pipeRadiusEnd) / (float)CurveSegmentCount;

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
        this.positionOfAvatar = transform.InverseTransformPoint(worldPositionOfAvatar);

        p0world = transform.TransformPoint(p0);
        p1world = transform.TransformPoint(p1);

        Vector3 normal = new Vector3();
        Vector3 point = new Vector3();
        //计算圆弧所在的平面
        Math3d.PlaneFrom3Points(out normal,out point, p0, p1,Vector3.zero);

        //计算车的位置在平面上的投影点
        Vector3 projectionPoint = Math3d.ProjectPointOnPlane(normal, point, positionOfAvatar);
        Vector3 pointCenter = projectionPoint.normalized * p0.magnitude;

        //转换为世界坐标输出
        pointOutput = transform.TransformPoint(pointCenter);
        Vector3 pointCenterDirection = new Vector3(
                1,
                -pointCenter.x/pointCenter.y,
                0);
        directionOutput = transform.TransformVector(pointCenterDirection).normalized;
        //计算progress
        //圆弧角度不大于270度即可
        float angle = angle_360(pointCenter, p0);
        progress = angle/angleOfPiple;
        if(angle > 90 && angle <= 270)
        {
            directionOutput = - directionOutput;
        }
        if(angle > 270 && angle <360)
        {
            progress = (angle-360)/angleOfPiple;
        }
    }

    public float GetPipeRadiusByProgress(float progress)
    {
        return pipeRadiusBegin * (1 - progress) + pipeRadiusEnd * progress;
    }

    public float GetPipeRadiusBySegmentIndex(int index)
    {
        return (pipeRadiusBegin * (curveSegmentCount - index) + pipeRadiusEnd * index) / curveSegmentCount;
    }

    public float GetPipeProgressBySegmentIndex(int index)
    {
        return (float)index/(float)CurveSegmentCount;
    }

    // given progress, get center track point in local coordinates
    public Vector3 GetCenterPointPositionByProgressLocal(float progress)
    {
        float angle = progress * angleOfPiple;
        Vector3 centerPoint = Quaternion.AngleAxis(angle, Vector3.back) * p0;
        return centerPoint;
    }

    // given progress, get center track point in global coordinates
    public Vector3 GetCenterPointPositionByProgressGlobal(float progress)
    {
        return transform.TransformPoint(GetCenterPointPositionByProgressLocal(progress));
    }

    // given progress, get center track point direction in global space
    public Vector3 GetCenterPointDirectionByProgressGlobal(float progress)
    {
        Vector3 centerPoint = GetCenterPointPositionByProgressLocal(progress);
        Vector3 centerPointDirection = new Vector3(
                1,
                -centerPoint.x / centerPoint.y,
                0);
        Vector3 directionOutput = transform.TransformVector(centerPointDirection).normalized;
        //圆弧角度不大于270度即可
        float angle = angle_360(centerPoint, p0);
        if (angle > 90 && angle <= 270)
        {
            directionOutput = -directionOutput;
        }
        return directionOutput;
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
        if(!Constant.showGizmos)return;
        Vector3 p0world;
        Vector3 p1world;

        //计算圆弧所在的平面

        // draw pipe beginning
        p0world = transform.TransformPoint(p0);
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(p0world, 0.5f);

        // draw pipe ending
        p1world = transform.TransformPoint(p1);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p1world, 0.5f);
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

    public Gradient coloring;
    private float time = 0;
    void FixedUpdate()
    {
        //定时改变颜色
        time += Time.deltaTime;
        if(time > 1f)time -= 1f;
        Color nextColor = coloring.Evaluate(time);
        renderer.material.SetColor("_EmissionColor", nextColor);
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

        float uStep = ringDistance / curveRadius; // one step along pipe curve
        curveAngle = uStep * curveSegmentCount * (360f / (2f * Mathf.PI));
        CreateFirstQuadRing(uStep);
        int iDelta = pipeSegmentCount * 4; // number of vertices in a single ring of the pipe
        for (int u = 2, i = iDelta; u <= curveSegmentCount; u++, i += iDelta)
        {
            CreateQuadRing(u * uStep, i, u);
        }
        mesh.vertices = vertices;
    }

    private void CreateQuadRing(float u, int i, int uNum)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount; // angle in radians of each ring segment
        int ringOffset = pipeSegmentCount * 4; // number of vertices in a single ring of the pipe

        Vector3 vertex = GetPointOnTorus(u, 0f, uNum);
        for (int v = 1; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertices[i - ringOffset + 2];
            vertices[i + 1] = vertices[i - ringOffset + 3];
            vertices[i + 2] = vertex;
            vertices[i + 3] = vertex = GetPointOnTorus(u, v * vStep, uNum);
        }
    }

    private void CreateFirstQuadRing(float u)
    {
        float vStep = (2f * Mathf.PI) / pipeSegmentCount;

        Vector3 vertexA = GetPointOnTorus(0f, 0f, 0);
        Vector3 vertexB = GetPointOnTorus(u, 0f, 1);
        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            vertices[i] = vertexA;
            vertices[i + 1] = vertexA = GetPointOnTorus(0f, v * vStep, 0);
            vertices[i + 2] = vertexB;
            vertices[i + 3] = vertexB = GetPointOnTorus(u, v * vStep, 1);
        }
    }

    private void SetUV()
    {
        float cellSize = 1f / 20f;
        uv = new Vector2[vertices.Length];

        // map the very first ring of the pipe
        Vector2 uvA = new Vector2(0f, 0f); // start at bottom left vertex
        Vector3 uvB = new Vector2(0f, cellSize); // start at top left vertex
        for (int v = 1, i = 0; v <= pipeSegmentCount; v++, i += 4)
        {
            uv[i] = uvA; // 0, 0
            uv[i + 1] = uvA = new Vector2(v * cellSize, 0f); // 1, 0
            uv[i + 2] = uvB; // 0, 1
            uv[i + 3] = uvB = new Vector2(v * cellSize, cellSize); // 1, 1
        }

        // map the remaining rings
        int ringOffset = pipeSegmentCount * 4; // number of vertices in a single ring of the pipe
        for (int u = 2, i = ringOffset; u <= CurveSegmentCount; u++)
        {
            uvA = uv[i - ringOffset + 2];
            uvB = new Vector2(uvA.x, uvA.y + cellSize);
            for (int v = 1; v <= pipeSegmentCount; v++, i += 4)
            {
                uv[i] = uvA;
                uv[i + 1] = uvA = new Vector2(v * cellSize, uvA.y);
                uv[i + 2] = uvB;
                uv[i + 3] = uvB = new Vector2(v * cellSize, uvB.y);
            }
        }

        //for (int i = 0; i < vertices.Length; i += 4)
        //{
        //    //uv[i] = Vector2.zero;
        //    //uv[i + 1] = Vector2.right;
        //    //uv[i + 2] = Vector2.up;
        //    //uv[i + 3] = Vector2.one;
        //    uv[i] = Vector2.zero;
        //    uv[i + 1] = new Vector2(cellSize, 0);
        //    uv[i + 2] = new Vector2(0, cellSize);
        //    uv[i + 3] = new Vector2(cellSize, cellSize);
        //}
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

    private int randomSegment;
    private int segmentRandomOffset = 6;
    public void AlignWith(Pipe pipe)
    {
        // random relative rotation
        randomSegment = Random.Range(segmentRandomOffset, curveSegmentCount-segmentRandomOffset);
        relativeRotation = randomSegment * 360f / pipeSegmentCount;

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
