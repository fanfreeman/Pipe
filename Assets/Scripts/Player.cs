using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;

    //public Avatar avatar;

    [HideInInspector]
    public Pipe currentPipe; // the current pipe the player is traveling in
    [HideInInspector]
    public Pipe prevPipe; // the pipe the player has just traveled through
    
    private Vector3 centerTrackPointPosition;
    protected Vector3 centerTrackPointDirection;

    [HideInInspector]
    public float progress = 0;

    public GameObject car;

    private const float ProgressDelta = 0.01f;

    Vector3 prevPosition;
    float accumulatedTime = 0;

    void Start()
    {
        currentPipe = pipeSystem.GetSecondPipe();
        prevPipe = pipeSystem.GetVeryFirstPipe();
        centerTrackPointPosition = currentPipe.cameraSpline.GetPoint(0);

        prevPosition = Vector3.zero;
    }

    protected virtual void Update()
    {
        // find distance between center track point and avatar
        float distanceToAvatar = Vector3.Distance(centerTrackPointPosition, transform.position);
        float newDistanceToAvatarUp = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress + ProgressDelta), transform.position);
        float newDistanceToAvatarDown = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress - ProgressDelta), transform.position);

        if (newDistanceToAvatarUp < newDistanceToAvatarDown)
        {
            while (newDistanceToAvatarUp < distanceToAvatar)
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
                    }
                    else {
                        prevPipe = currentPipe;
                        currentPipe = pipeSystem.GetSecondPipe();
                    }
                    progress -= 1f;
                }

                centerTrackPointPosition = currentPipe.cameraSpline.GetPoint(progress);
                distanceToAvatar = newDistanceToAvatarUp;
                newDistanceToAvatarUp = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress + ProgressDelta), transform.position);
            }
        } else {
            while (newDistanceToAvatarDown < distanceToAvatar)
            {
                // move track hook by setting a new value for progress along the spline
                progress -= ProgressDelta;

                // check if we have traveled the entire length of a pipe segment
                // and should set up the next pipe
                if (progress < 0)
                {
                    currentPipe = prevPipe;
                    progress += 1f;
                }

                centerTrackPointPosition = currentPipe.cameraSpline.GetPoint(progress);
                distanceToAvatar = newDistanceToAvatarDown;
                newDistanceToAvatarDown = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress - ProgressDelta), transform.position);
            }
        }


        //transform.LookAt(centerTrackPointDirection);
        //transform.Rotate(90f, 0, 0);
        
        //Vector3 moveDirection = centerTrackPointDirection;
        //accumulatedTime += Time.deltaTime;
        //if (accumulatedTime > 1f)
        //{
        //    prevPosition = transform.position;
        //    accumulatedTime = 0;
        //}
        //var newRot = Quaternion.LookRotation(moveDirection);
        //car.transform.rotation = Quaternion.Lerp(car.transform.rotation, newRot, Time.deltaTime * 10f);
    }

    void FixedUpdate()
    {
        //transform.LookAt(centerTrackPointPosition + centerTrackPointDirection);

        // apply force to move forward
        centerTrackPointDirection = currentPipe.cameraSpline.GetVelocity(progress);
        GetComponent<Rigidbody>().AddForce(centerTrackPointDirection * 6f, ForceMode.Acceleration);



        // apply force to make avatar stick to wall
        Vector3 upVector = GetUpVector();
        float magnitudeModifier = (3f - upVector.magnitude) * 10f;
        GetComponent<Rigidbody>().AddForce(-upVector * magnitudeModifier, ForceMode.Acceleration);

        // hover
        //Ray ray = new Ray(transform.position, -transform.up);
        //RaycastHit hit;
        //float hoverHeight = 1.0f;
        //float hoverForce = 50f;
        //if (Physics.Raycast(ray, out hit, hoverHeight))
        //{
        //    float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
        //    Vector3 appliedHoverForce = upVector * proportionalHeight * hoverForce;
        //    GetComponent<Rigidbody>().AddForce(appliedHoverForce, ForceMode.Acceleration);
        //}
    }

    private void OnDrawGizmos()
    {
        // draw the point on the center track closest to the avatar
        if (Application.isPlaying) Gizmos.DrawSphere(centerTrackPointPosition, 0.5f);
    }

    public Vector3 GetUpVector()
    {
        return centerTrackPointPosition - transform.position;
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
