using System.Collections;
using UnityEditor;
using UnityEngine;

public class PipeItemTurntable : PipeItem {

    private Transform rotater;
    private float radius;
    void Awake()
    {
        rotater = transform.GetChild(0);
        radius = rotater.GetComponent<MeshRenderer>().bounds.size.y/2;
    }

    public override void Position(Pipe pipe, int segment, float ringRotation, float pipeRadius)
    {
        rotater.gameObject.SetActive(true);
        transform.SetParent(pipe.transform, false);

        float curveRotation = segment * pipe.CurveAngle / pipe.CurveSegmentCount;
        progress = pipe.GetPipeProgressBySegmentIndex(segment);

        rotater.localPosition = new Vector3(0f, pipe.CurveRadius); // put item in the pipe
        rotater.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
        rotater.localScale = Vector3.one * pipeRadius/(radius+0.02f);
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);
    }

    private float progress;

}