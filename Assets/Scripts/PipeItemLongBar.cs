using UnityEngine;
using System.Collections;

public class PipeItemLongBar : PipeItem {

    private Transform rotater;
    private Transform item;
    private void Awake()
    {
        rotater = transform.GetChild(0);
        item = rotater.GetChild(0);
    }

    public override void Position(Pipe pipe, float curveRotation, float ringRotation)
    {


        //根据管道改变scale
        float y =
            item.GetComponent<Collider>().bounds.size.y;

        item.localScale = new Vector3(
                item.localScale.x,
                ((2f * pipe.pipeRadius)/y),
                item.localScale.z
        );
        transform.SetParent(pipe.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);

        rotater.localPosition = new Vector3(0f, pipe.CurveRadius);
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);
    }
}
