using UnityEngine;
using System.Collections;

/**
 * Calculate camera direction using a moving average of center point directions
 */
public class CameraController : MonoBehaviour {

    public PipeSystem pipeSystem;

    private Player player;

    private const int NumDataPoints = 5;
    private Vector3[] centerTrackPointPositions = new Vector3[NumDataPoints];
    private Vector3[] centerTrackPointDirections = new Vector3[NumDataPoints];
    private float[] progresses = new float[NumDataPoints];
    private Pipe[] currentPipes = new Pipe[NumDataPoints];

    public GameObject vehicleAndCameraObject;

    public GameObject vehicleObject;
    
    void Start()
    {
        player = GetComponent<Player>();

        for (int i = 0; i < NumDataPoints; i++)
        {
            currentPipes[i] = pipeSystem.GetStartingPipe();
            progresses[i] = i * (1f / NumDataPoints);
        }
    }

    private void Update()
    {
        Vector3 avatarPosition = player.GetAvatarPosition();
        //Vector3 forwardPosition = avatarPosition + player.GetCenterTrackPointDirection() * 5f; // a point in front of the player
        // find progress and pipe for first data point
        currentPipes[0] = player.currentPipe;
        currentPipes[0].GetPlaneOfCurve(
                avatarPosition,
                ref centerTrackPointDirections[0],
                ref centerTrackPointPositions[0],
                ref progresses[0]
        );
        
        // find progress and pipe for last data point
        progresses[NumDataPoints - 1] = progresses[0] + (NumDataPoints - 1) * (1f / NumDataPoints);
        if (currentPipes[NumDataPoints - 1] != currentPipes[0]) progresses[NumDataPoints - 1] -= 1f;
        if (progresses[NumDataPoints - 1] >= 1f)
        {
            currentPipes[NumDataPoints - 1] = pipeSystem.SetupNextPipe();
            progresses[NumDataPoints - 1] -= 1f;
        }

        // find progress and pipe for all intermediate data points
        for (int i = 1; i < NumDataPoints - 1; i++)
        {
            progresses[i] = progresses[0] + i * (1f / NumDataPoints);
            if (currentPipes[i] != currentPipes[0]) progresses[i] -= 1f;
            if (progresses[i] >= 1f)
            {
                currentPipes[i] = currentPipes[NumDataPoints - 1];
                progresses[i] -= 1f;
            }
        }

        // find center track points and directions for all data points
        for (int i = 1; i < NumDataPoints; i++)
        {
            centerTrackPointPositions[i] = currentPipes[i].GetCenterPointPositionByProgressGlobal(progresses[i]);
            centerTrackPointDirections[i] = currentPipes[i].GetCenterPointDirectionByProgressGlobal(progresses[i]);
        }
        
        // update camera position
        vehicleAndCameraObject.transform.position = avatarPosition;

        // update camera direction
        //Vector3 cameraTarget = centerTrackPointPosition + centerTrackPointDirection * 5f;
        //Vector3 cameraTarget = currentPipe.GetCenterPointByProgressGlobal(progress + 0.4f);

        // find the average of all center track point directions
        Vector3 sumDirections = Vector3.zero;
        for (int i = 0; i < NumDataPoints; i++)
        {
            sumDirections += centerTrackPointDirections[i];
        }
        Vector3 averageDirection = sumDirections / (float)NumDataPoints;
        Vector3 cameraDirection = averageDirection;
        //Vector3 cameraDirection = centerTrackPointDirections[0];
        Quaternion cameraRotation = Quaternion.LookRotation(cameraDirection, player.GetUpVector());
        //Quaternion cameraRotation = Quaternion.LookRotation(centerTrackPointDirection, GetUpVector());
        //Debug.Log(cameraRotation.eulerAngles.ToString());
        //coolVehicle.transform.rotation = cameraRotation;
        vehicleAndCameraObject.transform.rotation = Quaternion.RotateTowards(vehicleAndCameraObject.transform.rotation, cameraRotation, Time.deltaTime * 100f);
        //iTween.RotateUpdate(coolVehicle, iTween.Hash("rotation", cameraRotation.eulerAngles, "time", 1f));
        
        // update vehicle direction
        Vector3 vehicleDirection = player.GetCenterTrackPointDirection();
        Quaternion vehicleRotation = Quaternion.LookRotation(vehicleDirection, player.GetUpVector());
        vehicleObject.transform.rotation = Quaternion.RotateTowards(vehicleObject.transform.rotation, vehicleRotation, Time.deltaTime * 300f);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < NumDataPoints; i ++)
            {
                // draw center track points
                Gizmos.DrawSphere(centerTrackPointPositions[i], 0.3f);

                // draw center track point directions
                Gizmos.DrawLine(centerTrackPointPositions[i], centerTrackPointPositions[i] + centerTrackPointDirections[i] * 6f);
            }
        }
    }
}
