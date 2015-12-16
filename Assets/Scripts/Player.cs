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

    private Pipe currentPipe;

    private float distanceTraveled;

    private float deltaToRotation; // system rotation speed; delta times this equals system rotation
    private float systemRotation; // rotation of the entire pipe system

    private Transform world, rotater;

    private float worldRotation, avatarRotation;

    private Vector3 centerTrackPosition;
    private float progress = 0;

    public void StartGame(int accelerationMode)
    {
        distanceTraveled = 0f;
        avatarRotation = 0f;
        systemRotation = 0f;
        worldRotation = 0f;
        acceleration = accelerations[accelerationMode];
        velocity = startVelocity;
        currentPipe = pipeSystem.SetupFirstPipe();
        SetupCurrentPipe();
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
        centerTrackPosition = pipeSystem.cameraSpline.GetPoint(0);
    }

    private void Update()
    {
        //velocity += acceleration * Time.deltaTime;

        //float delta = velocity * Time.deltaTime;
        //distanceTraveled += delta;
        ////systemRotation += delta * deltaToRotation;

        //// check if we have traveled the entire length of a pipe segment
        //// and should set up the next pipe
        //if (systemRotation >= currentPipe.CurveAngle)
        //{
        //    delta = (systemRotation - currentPipe.CurveAngle) / deltaToRotation;
        //    currentPipe = pipeSystem.SetupNextPipe();
        //    SetupCurrentPipe();
        //    systemRotation = delta * deltaToRotation;
        //}

        //pipeSystem.transform.localRotation =
        //    Quaternion.Euler(0f, 0f, systemRotation);

        // find distance between center track point and avatar
        float distanceToAvatar = Vector3.Distance(centerTrackPosition, avatar.transform.position);
        while (distanceToAvatar > 2f)
        {
            // move camera by setting a new value for progress along the spline
            progress += 0.001f;
            if (progress > 1f)
            {
                progress = 1f;
                break;
            }
            centerTrackPosition = pipeSystem.cameraSpline.GetPoint(progress);
            distanceToAvatar = Vector3.Distance(centerTrackPosition, avatar.transform.position);
        }

        Vector3 forceDirection = pipeSystem.cameraSpline.GetVelocity(progress);
        avatar.GetComponent<Rigidbody>().AddForce(forceDirection * 2f, ForceMode.Acceleration);
        
        UpdateAvatarRotation();
        hud.SetValues(distanceTraveled, velocity);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(centerTrackPosition, 0.5f);
    }

    public Vector3 GetUpVector()
    {
        return centerTrackPosition - avatar.transform.position;
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

    public void Die()
    {
        mainMenu.EndGame(distanceTraveled);
        gameObject.SetActive(false);
    }
}
