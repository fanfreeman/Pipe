using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PipeSystem pipeSystem;
    public Player player;

    private float progress = 0;

    private Vector3 targetPosition;

    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        targetPosition = player.currentPipe.cameraSpline.GetPoint(0);
    }

    private void Update()
    {
        //// find distance between camera and avatar
        //float distanceToAvatar = Vector3.Distance(targetPosition, player.avatar.transform.position);
        //while (distanceToAvatar > 4f)
        //{
        //    // move camera by setting a new value for progress along the spline
        //    progress += 0.001f;
        //    if (progress > 1f)
        //    {
        //        progress = 1f;
        //        break;
        //    }
        //    targetPosition = pipeSystem.cameraSpline.GetPoint(progress);
        //    distanceToAvatar = Vector3.Distance(targetPosition, player.avatar.transform.position);
        //}


        //transform.position = targetPosition;
        targetPosition = player.GetCenterTrackHookPosition();
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.3f);

        // make camera look at avatar
        //transform.LookAt(player.avatar.transform, player.GetUpVector());
        //Vector3 forwardVector = pipeSystem.cameraSpline.GetPoint(progress + 0.03f) - transform.position;
        Vector3 forwardVector = player.GetCenterTrackPointDirection();
        var newRot = Quaternion.LookRotation(forwardVector);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 2f);
    }
}
