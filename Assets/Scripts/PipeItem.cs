using UnityEngine;
using System.Collections;

public abstract class PipeItem : MonoBehaviour {

    public bool isNewTon;
    public abstract void Position(Pipe pipe, float curveRotation, float ringRotation);
}
