using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;

    public Avatar avatar;

    [HideInInspector]
    public Pipe currentPipe; // the current pipe the player is traveling in
    [HideInInspector]
    public Pipe prevPipe; // the pipe the player has just traveled through
    
    private Vector3 centerTrackPointPosition;
    protected Vector3 centerTrackPointDirection;

    [HideInInspector]
    public float progress = 0;

    private const float ProgressDelta = 0.01f;

    void Start()
    {
        currentPipe = pipeSystem.GetSecondPipe();
        prevPipe = pipeSystem.GetVeryFirstPipe();
        centerTrackPointPosition = currentPipe.cameraSpline.GetPoint(0);
    }

    protected virtual void Update()
    {
        // find distance between center track point and avatar
        float distanceToAvatar = Vector3.Distance(centerTrackPointPosition, avatar.transform.position);
        float newDistanceToAvatar = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress + ProgressDelta), avatar.transform.position);

        while (newDistanceToAvatar < distanceToAvatar)
        {
            // move track hook by setting a new value for progress along the spline
            progress += ProgressDelta;

            // check if we have traveled the entire length of a pipe segment
            // and should set up the next pipe
            if (progress > 1f)
            {
                if (currentPipe.id > pipeSystem.idOfPreviousPipeTraveled)
                {
                    prevPipe = currentPipe;
                    currentPipe = pipeSystem.SetupNextPipe();
                    pipeSystem.idOfPreviousPipeTraveled++;
                } else {
                    prevPipe = currentPipe;
                    currentPipe = pipeSystem.GetSecondPipe();
                }
                progress -= 1f;
            }

            centerTrackPointPosition = currentPipe.cameraSpline.GetPoint(progress);
            distanceToAvatar = newDistanceToAvatar;
            newDistanceToAvatar = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress + ProgressDelta), avatar.transform.position);
        }

        // apply force to move forward
        centerTrackPointDirection = currentPipe.cameraSpline.GetVelocity(progress);
        avatar.GetComponent<Rigidbody>().AddForce(centerTrackPointDirection * 5f, ForceMode.Acceleration);

        // apply force to make avatar stick to wall
        Vector3 upVector = GetUpVector();
        //Debug.Log(upVector.magnitude);
        float magnitudeModifier = (3f - upVector.magnitude) * 10f;
        avatar.GetComponent<Rigidbody>().AddForce(-upVector * magnitudeModifier, ForceMode.Acceleration);
    }

    private void OnDrawGizmos()
    {
        // draw the point on the center track closest to the avatar
        Gizmos.DrawSphere(centerTrackPointPosition, 0.5f);
    }

    public Vector3 GetUpVector()
    {
        return centerTrackPointPosition - avatar.transform.position;
    }

    private void UpdateAvatarRotation()
    {
        float rotationInput = 0f;
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                if (Input.GetTouch(0).position.x < Screen.width * 0.5f)
                {
                    rotationInput = -1f;
                }
                else {
                    rotationInput = 1f;
                }
            }
        }
        else {
            rotationInput = Input.GetAxis("Horizontal");
        }

        //avatarRotation +=
        //    rotationVelocity * Time.deltaTime * rotationInput;
        //if (avatarRotation < 0f)
        //{
        //    avatarRotation += 360f;
        //}
        //else if (avatarRotation >= 360f)
        //{
        //    avatarRotation -= 360f;
        //}
        //rotater.localRotation = Quaternion.Euler(avatarRotation, 0f, 0f);
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetCenterTrackHookPosition()
    {
        return centerTrackPointPosition;
    }

    public Vector3 GetCenterTrackPointDirection()
    {
        return centerTrackPointDirection;
    }
}
