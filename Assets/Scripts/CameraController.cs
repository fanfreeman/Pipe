using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PipeSystem pipeSystem;

    private Player player;

    private Vector3 centerTrackPointPosition = Vector3.zero;
    private Vector3 centerTrackPointDirection = Vector3.zero;
    private float progress = 0;

    private Pipe currentPipe;

    public GameObject vehicleAndCameraObject;

    void Start()
    {
        player = GetComponent<Player>();
        currentPipe = pipeSystem.GetStartingPipe();
    }

    private void Update()
    {
        Vector3 avatarPosition = player.GetAvatarPosition();
        Vector3 forwardPosition = avatarPosition + player.GetCenterTrackPointDirection() * 5f; // a point in front of the player
        currentPipe.GetPlaneOfCurve(
                forwardPosition,
                ref centerTrackPointDirection,
                ref centerTrackPointPosition,
                ref progress
        );

        if (progress >= 1) currentPipe = pipeSystem.SetupNextPipe();

        // update camera position
        vehicleAndCameraObject.transform.position = avatarPosition;

        // update camera rotation
        //Vector3 cameraTarget = centerTrackPointPosition + centerTrackPointDirection * 5f;
        //Vector3 cameraTarget = currentPipe.GetCenterPointByProgressGlobal(progress + 0.4f);
        Vector3 cameraDirection = centerTrackPointDirection;
        Quaternion cameraRotation = Quaternion.LookRotation(cameraDirection, player.GetUpVector());
        //Quaternion cameraRotation = Quaternion.LookRotation(centerTrackPointDirection, GetUpVector());
        //Debug.Log(cameraRotation.eulerAngles.ToString());
        //coolVehicle.transform.rotation = cameraRotation;
        vehicleAndCameraObject.transform.rotation = Quaternion.RotateTowards(vehicleAndCameraObject.transform.rotation, cameraRotation, Time.deltaTime * 300f);
        //iTween.RotateUpdate(coolVehicle, iTween.Hash("rotation", cameraRotation.eulerAngles, "time", 1f));
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            // draw center track point
            Gizmos.DrawSphere(centerTrackPointPosition, 0.3f);

            // draw center track point direction
            Gizmos.DrawLine(centerTrackPointPosition, centerTrackPointPosition + centerTrackPointDirection * 6f);
        }
    }
}
