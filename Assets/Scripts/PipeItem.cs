using UnityEngine;
using System.Collections;

public class PipeItem : MonoBehaviour {

    private Transform rotater;

    private void Awake()
    {
        rotater = transform.GetChild(0);
    }

    public void Position(Pipe pipe, float curveRotation, float ringRotation, float pipeRadius)
    {
        transform.SetParent(pipe.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
        rotater.localPosition = new Vector3(0f, pipe.CurveRadius); // put item in the pipe
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);

        Transform rotaterChild = rotater.GetChild(0); // the actual model of the item
        rotaterChild.localPosition = new Vector3(0f, -pipeRadius); // push item to the wall
    }
}
