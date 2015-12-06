using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;

    public float velocity;

    private Pipe currentPipe;

    private float distanceTraveled;

    private float deltaToRotation;
    private float systemRotation;

    private Transform world;

    private float worldRotation;

    private void Start()
    {
        world = pipeSystem.transform.parent;
        currentPipe = pipeSystem.SetupFirstPipe();
        SetupCurrentPipe();
    }

    private void Update()
    {
        float delta = velocity * Time.deltaTime;
        distanceTraveled += delta;
        systemRotation += delta * deltaToRotation;

        // check if should set up next pipe
        if (systemRotation >= currentPipe.CurveAngle)
        {
            delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
            currentPipe = pipeSystem.SetupNextPipe();
            SetupCurrentPipe();
            systemRotation = delta * deltaToRotation;
        }

        pipeSystem.transform.localRotation =
            Quaternion.Euler(0f, 0f, systemRotation);
    }

    private void SetupCurrentPipe()
    {
        deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
        worldRotation += currentPipe.RelativeRotation;
        if (worldRotation < 0f)
        {
            worldRotation += 360f;
        }
        else if (worldRotation >= 360f)
        {
            worldRotation -= 360f;
        }
        world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
    }
}
