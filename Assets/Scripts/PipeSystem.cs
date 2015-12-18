using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PipeSystem : MonoBehaviour {

    public Pipe pipePrefab;

    public int pipeCount;

    public int emptyPipeCount;

    private Pipe[] pipes;

    private void Awake()
    {
        // instantiate pipes
        pipes = new Pipe[pipeCount];
        for (int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i] = Instantiate<Pipe>(pipePrefab);
            pipe.transform.SetParent(transform, false);
        }
    }

    public Pipe SetupFirstPipe()
    {
        // generate all the pipes in the system
        for (int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i];
            pipe.Generate(i > emptyPipeCount);
            if (i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
            }
        }
        //AlignNextPipeWithOrigin();

        // move the opening of the first pipe to world origin
        transform.localPosition = new Vector3(0f, -pipes[0].CurveRadius);
        return pipes[0];
    }

    // reuse pipes by moving a pipe just traveled through to the end of the system
    public Pipe SetupNextPipe()
    {
        ShiftPipes();
        //AlignNextPipeWithOrigin();
        pipes[pipes.Length - 1].Generate();
        pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2]);
        //transform.localPosition = new Vector3(0f, -pipes[0].CurveRadius);
        return pipes[0];
    }

    // move pipes[0] to the end of the array
    private void ShiftPipes()
    {
        Pipe temp = pipes[0];
        for (int i = 1; i < pipes.Length; i++)
        {
            pipes[i - 1] = pipes[i];
        }
        pipes[pipes.Length - 1] = temp;
    }

    //private void AlignNextPipeWithOrigin()
    //{
    //    Transform transformToAlign = pipes[0].transform;
    //    for (int i = 1; i < pipes.Length; i++)
    //    {
    //        pipes[i].transform.SetParent(transformToAlign);
    //    }

    //    transformToAlign.localPosition = Vector3.zero;
    //    transformToAlign.localRotation = Quaternion.identity;

    //    for (int i = 1; i < pipes.Length; i++)
    //    {
    //        pipes[i].transform.SetParent(transform);
    //    }
    //}
}
