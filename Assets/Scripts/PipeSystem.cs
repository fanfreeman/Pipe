using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PipeSystem : MonoBehaviour {

    public Pipe pipePrefab;

    public int pipeCount;

    public int emptyPipeCount;

    private Pipe[] pipes;

    private const int StartingPipeIndex = 1;

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

    // set up all initial pipes and return the second pipe
    public Pipe SetupInitialPipes()
    {
        // generate all initial pipes in the system
        for (int i = 0; i < pipes.Length; i++)
        {
            Pipe pipe = pipes[i];

            float prevPipeEndRadius = 2f;
            if (i > 0) prevPipeEndRadius = pipes[i - 1].GetPipeEndRadius();
            pipe.Generate(prevPipeEndRadius, i > emptyPipeCount);
            if (i > 0)
            {
                pipe.AlignWith(pipes[i - 1]);
                pipes[i - 1].nextPipe = pipes[i]; // set next pipe
            }
        }
        AlignNextPipeWithOrigin();

        // move the opening of the first pipe to world origin
        transform.localPosition = new Vector3(0f, -pipes[StartingPipeIndex].CurveRadius);
        return pipes[StartingPipeIndex];
    }

    public Pipe GetStartingPipe()
    {
        return pipes[StartingPipeIndex];
    }

    // return the very first pipe in the system
    public Pipe GetVeryFirstPipe()
    {
        return pipes[0];
    }

    // reuse pipes by moving a pipe just traveled through to the end of the system
    public Pipe SetupNextPipe()
    {
        ShiftPipes();
        //AlignNextPipeWithOrigin();
        pipes[pipes.Length - 1].Generate(pipes[pipes.Length - 2].GetPipeEndRadius());
        pipes[pipes.Length - 1].AlignWith(pipes[pipes.Length - 2]);
        //transform.localPosition = new Vector3(0f, -pipes[0].CurveRadius);
        pipes[pipes.Length - 2].nextPipe = pipes[pipes.Length - 1]; // set next pipe
        return pipes[StartingPipeIndex];
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

    // make all other pipes children of pipes[StartingPipeIndex]
    // and put pipes[StartingPipeIndex] at world origin
    private void AlignNextPipeWithOrigin()
    {
        Transform transformToAlign = pipes[StartingPipeIndex].transform;
        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != StartingPipeIndex) pipes[i].transform.SetParent(transformToAlign);
        }

        transformToAlign.localPosition = Vector3.zero;
        transformToAlign.localRotation = Quaternion.identity;

        for (int i = 0; i < pipes.Length; i++)
        {
            if (i != StartingPipeIndex) pipes[i].transform.SetParent(transform);
        }
    }
}
