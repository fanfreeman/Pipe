using UnityEngine;
using System.Collections;

public class PipeItemGeneral : PipeItem {

    private Transform rotater;

    void Awake()
    {
        rotater = transform.GetChild(0);
    }

    public override void Position(Pipe pipe, int segment, float ringRotation, float pipeRadius)
    {
        float curveRotation = segment * pipe.CurveAngle / pipe.CurveSegmentCount;
        transform.SetParent(pipe.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
        rotater.localPosition = new Vector3(0f, pipe.CurveRadius); // put item in the pipe
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);

        Transform rotaterChild = rotater.GetChild(0); // the actual model of the item
        rotaterChild.localPosition = new Vector3(0f, -pipeRadius); // push item to the wall
    }
}

