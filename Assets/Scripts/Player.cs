using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;

    public float rotationVelocity;

    public MainMenu mainMenu;

    public float startVelocity;

    public float[] accelerations;

    public HUD hud;

    public Avatar avatar;

    private float acceleration, velocity;

    public Pipe currentPipe; // the current pipe the player is traveling in
    public Pipe prevPipe; // the pipe the player has just traveled through

    private float distanceTraveled;

    private Transform world, rotater;

    private float worldRotation, avatarRotation;
    
    private Vector3 centerTrackPointPosition;
    private Vector3 centerTrackPointDirection;
    private float progress = 0;

    private float progressDelta;

    public void StartGame(int accelerationMode)
    {
        distanceTraveled = 0f;
        avatarRotation = 0f;
        worldRotation = 0f;
        acceleration = accelerations[accelerationMode];
        velocity = startVelocity;
        currentPipe = pipeSystem.SetupInitialPipes();
        prevPipe = pipeSystem.GetVeryFirstPipe();
        //SetupCurrentPipe();
        gameObject.SetActive(true);
        hud.SetValues(distanceTraveled, velocity);
    }

    private void Awake()
    {
        world = pipeSystem.transform.parent;
        rotater = transform.GetChild(0);
        //gameObject.SetActive(false);
    }

    void Start()
    {
        StartGame(0);
        centerTrackPointPosition = currentPipe.cameraSpline.GetPoint(0);
    }

    private void Update()
    {
        //pipeSystem.transform.localRotation =
        //    Quaternion.Euler(0f, 0f, systemRotation);

        // find distance between center track point and avatar
        float progressDelta = 0.01f;
        float distanceToAvatar = Vector3.Distance(centerTrackPointPosition, avatar.transform.position);
        float newDistanceToAvatar = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress + progressDelta), avatar.transform.position);

        while (newDistanceToAvatar < distanceToAvatar)
        {
            // move track hook by setting a new value for progress along the spline
            progress += progressDelta;

            // check if we have traveled the entire length of a pipe segment
            // and should set up the next pipe
            if (progress > 1f)
            {
                prevPipe = currentPipe;
                currentPipe = pipeSystem.SetupNextPipe();
                progress -= 1f;
            }

            centerTrackPointPosition = currentPipe.cameraSpline.GetPoint(progress);
            distanceToAvatar = newDistanceToAvatar;
            newDistanceToAvatar = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress + progressDelta), avatar.transform.position);
        }

        // apply force to move forward
        centerTrackPointDirection = currentPipe.cameraSpline.GetVelocity(progress);
        avatar.GetComponent<Rigidbody>().AddForce(centerTrackPointDirection * 5f, ForceMode.Acceleration);

        // apply force to make avatar stick to wall
        Vector3 upVector = GetUpVector();
        Debug.Log(upVector.magnitude);
        float magnitudeModifier = (3f - upVector.magnitude) * 10f;
        avatar.GetComponent<Rigidbody>().AddForce(-upVector * magnitudeModifier, ForceMode.Acceleration);

        //UpdateAvatarRotation();
        //hud.SetValues(distanceTraveled, velocity);
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

    //private void SetupCurrentPipe()
    //{
    //    deltaToRotation = 360f / (2f * Mathf.PI * currentPipe.CurveRadius);
    //    worldRotation += currentPipe.RelativeRotation;
    //    if (worldRotation < 0f)
    //    {
    //        worldRotation += 360f;
    //    }
    //    else if (worldRotation >= 360f)
    //    {
    //        worldRotation -= 360f;
    //    }
    //    world.localRotation = Quaternion.Euler(worldRotation, 0f, 0f);
    //}

    public void Die()
    {
        mainMenu.EndGame(distanceTraveled);
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
