using System.Collections;
using UnityEditor;
using UnityEngine;

public class PipeItemTurntable : PipeItem {
    public GameObject tester;
    private Material testerM;

    private Transform rotater;
    private float radius;
    void Awake()
    {
        rotater = transform.GetChild(0);
        radius = rotater.GetComponent<MeshRenderer>().bounds.size.y/2;
 //       Debug.Log("Awake radius:"+radius);

        testerM = tester.GetComponent<SkinnedMeshRenderer>().material;
    }

    public override void Position(Pipe pipe, float curveRotation, float ringRotation, float pipeRadius)
    {
        this.pipe = pipe;



        rotater.gameObject.SetActive(true);
        transform.SetParent(pipe.transform, false);

        int index = Mathf.CeilToInt(curveRotation/(pipe.CurveAngle / pipe.CurveSegmentCount));

        rotater.localScale = Vector3.one * (pipeRadius/radius);

      //  rotater.localRotation = Quaternion.Euler(0f, 0f, );
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, -curveRotation);

        progress = pipe.GetPipeProgressBySegmentIndex(index);
        Debug.Log("index:"+index+"  progress:"+pipe.GetPipeProgressBySegmentIndex(index) +
        "  localPosition:"+pipe.GetCenterPointByProgressLocal(
                pipe.GetPipeProgressBySegmentIndex(index)
        ).ToString());

        rotater.localPosition =
        pipe.GetCenterPointByProgressLocal(
                pipe.GetPipeProgressBySegmentIndex(index)
        );

        testerM.SetColor("_Color",Color.white * progress);

    }

    private float progress;
    private Pipe pipe;

    void OnDrawGizmos()
    {
        if(Application.isPlaying && pipe != null)
        {
            Gizmos.color = Color.white * progress;
            Gizmos.DrawSphere(pipe.GetCenterPointByProgressGlobal(progress),0.3f);
        }
    }
}