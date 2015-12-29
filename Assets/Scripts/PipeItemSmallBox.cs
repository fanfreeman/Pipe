using UnityEngine;
using System.Collections;

public class PipeItemSmallBox : PipeItem {

    private Transform rotater;
    private Transform item;

    private void Awake()
    {
        rotater = transform.GetChild(0);
        item = rotater.transform.GetChild(0);
    }

    public override void Position(Pipe pipe, float curveRotation, float ringRotation)
    {
        transform.SetParent(pipe.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
        item.localPosition = new Vector3(0f, -pipe.pipeRadius, 0f);
        rotater.localPosition = new Vector3(0f, pipe.CurveRadius);
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);
    }
}
