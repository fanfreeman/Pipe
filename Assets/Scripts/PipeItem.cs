using UnityEngine;
using System.Collections;

public abstract class PipeItem : MonoBehaviour {
    public abstract void Position(Pipe pipe, int segment, float ringRotation, float pipeRadius);
}