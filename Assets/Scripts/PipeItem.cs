﻿using UnityEngine;
using System.Collections;

public abstract class PipeItem : MonoBehaviour {
    public abstract void Position(Pipe pipe, float curveRotation, float ringRotation, float pipeRadius);
}