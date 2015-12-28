using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    public PipeSystem pipeSystem;
    public Player player;

    private float progress = 0;

    private Vector3 targetPosition;

    private Vector3 velocity = Vector3.zero;

    private Pipe currentPipe;

    void Start()
    {
        // start camera near end of very first pipe
        currentPipe = player.prevPipe;
        progress = 0.9f;
        targetPosition = currentPipe.cameraSpline.GetPoint(progress);
        transform.position = targetPosition;
    }

    private void Update()
    {
        // find distance between camera and avatar
        float distanceToAvatar = Vector3.Distance(targetPosition, player.avatar.transform.position);
        while (distanceToAvatar > 2.1f)
        {
            // move camera by setting a new value for progress along the spline
            progress += Time.deltaTime * 0.1f;
            if (progress > 1f)
            {
                currentPipe = player.currentPipe;
                progress = 0;
            }
            targetPosition = currentPipe.cameraSpline.GetPoint(progress);
            distanceToAvatar = Vector3.Distance(targetPosition, player.avatar.transform.position);
        }

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, Time.deltaTime * 10f);
        //transform.position = targetPosition;

        // update camera rotation by look at avatar
        //transform.LookAt(player.avatar.transform, player.GetUpVector());
        //Vector3 forwardVector = pipeSystem.cameraSpline.GetPoint(progress + 0.03f) - transform.position;
        //var newRot = Quaternion.LookRotation(forwardVector);
        //transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 2f);

        // update camera rotation using center track point direction
        //Vector3 forwardVector = player.GetCenterTrackPointDirection();
        Vector3 forwardVector = player.GetCenterTrackHookPosition() + player.GetCenterTrackPointDirection() * 2 - transform.position;
        var newRot = Quaternion.LookRotation(forwardVector, player.GetUpVector());
        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 10f);
        //transform.rotation = newRot;
    }
}
