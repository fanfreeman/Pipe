using UnityEngine;
using System.Collections;

public class PipeItemRabbit : PipeItem {

    private Transform rotater;
    public  Animation animation;
    private float length;

    private AnimationClip animationClip;
    private float waitingTimeUpper = 0.8f;
    private float waitingFloor = 5.8f;

    void Awake()
    {
        rotater = transform.GetChild(0);

        animationClip =animation.GetClip("Loop_Animation");
        animationClip.SampleAnimation(animation.gameObject, 0f);

        length = animationClip.length;
    }

    void Start()
    {
        //等随机的时间后播放动画
        StartCoroutine(WaitForAWhile());
    }

    IEnumerator WaitForAWhile()
    {
        yield return new WaitForSeconds(Random.Range(waitingTimeUpper,
                waitingFloor));
        StartCoroutine(UpdateCoutine());
    }


    IEnumerator UpdateCoutine()
    {
        animation.Play();
        yield return new WaitForSeconds(length + Random.Range(waitingTimeUpper,
                waitingFloor));
        StartCoroutine(UpdateCoutine());
    }

    public override void Position(Pipe pipe, int segment, float ringRotation, float pipeRadius)
    {
        float curveRotation = segment * pipe.CurveAngle / pipe.CurveSegmentCount;
        ringRotation =
        (Random.Range(0, pipe.pipeSegmentCount)) *
            360f / pipe.pipeSegmentCount;
        transform.SetParent(pipe.transform, false);
        transform.localRotation = Quaternion.Euler(0f, 0f, -curveRotation);
        rotater.localPosition = new Vector3(0f, pipe.CurveRadius); // put item in the pipe
        rotater.localRotation = Quaternion.Euler(ringRotation, 0f, 0f);

        Transform rotaterChild = rotater.GetChild(0); // the actual model of the item
        rotaterChild.localPosition = new Vector3(0f, -pipeRadius + 0.1f); // push item to the wall
    }
}


