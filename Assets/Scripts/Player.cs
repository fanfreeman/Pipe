using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public PipeSystem pipeSystem;
    public GameObject coolVehicle;

    public float rotationVelocity;
    public MainMenu mainMenu;

    public float floatingHeight;

    public float startVelocity;

    public float[] accelerations;

    public float steeringForce = 20f;

    public HUD hud;

    public Avatar avatar;

    private float acceleration, velocity;

    public Pipe currentPipe; // the current pipe the player is traveling in
    public Pipe prevPipe; // the pipe the player has just traveled through

    private float distanceTraveled;

    private Transform world, rotater;

    private float worldRotation, avatarRotation;
    
    private Vector3 centerTrackPointPosition = Vector3.zero;
    private Vector3 centerTrackPointDirection = Vector3.zero;
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
        currentPipe.GetPlaneOfCurve(
                transform.TransformPoint(avatar.transform.position),
                ref centerTrackPointDirection,
                ref centerTrackPointPosition,
                ref progress
        );
    }

    private void Update()
    {
        currentPipe.GetPlaneOfCurve(
                avatar.transform.position,
                ref centerTrackPointDirection,
                ref centerTrackPointPosition,
                ref progress
        );

        if (progress >= 1) currentPipe = pipeSystem.SetupNextPipe();

        // apply force to move forward
        avatar.GetComponent<Rigidbody>().AddForce(centerTrackPointDirection * 15f, ForceMode.Acceleration);

        float currentPipeRadius = currentPipe.GetPipeRadiusByProgress(progress);

        // apply force to make avatar stick to wall
        Vector3 upVector = GetUpVector();
        //float magnitudeModifier = (currentPipeRadius - upVector.magnitude + 1f) * 10f;
        avatar.GetComponent<Rigidbody>().AddForce(-upVector * 20f / currentPipeRadius, ForceMode.Acceleration);

        //Vector3 lookAt = avatar.transform.position;// Vector3.SmoothDamp(coolVehicle.transform.position, avatar.transform.position, ref coolVehicleLookAtVelocity, 0.03f);

        //Vector3 forwardVector =  avatar.transform.position - coolVehicle.transform.position;
        //var newRot = Quaternion.LookRotation(forwardVector ,upVector);
        //   coolVehicle.transform.rotation = Quaternion.Lerp(coolVehicle.transform.rotation, newRot, );


        //UpdateAvatarRotation();
        //hud.SetValues(distanceTraveled, velocity);

        // update camera position
        coolVehicle.transform.position = avatar.transform.position;

        // update camera rotation
        //Vector3 cameraTarget = centerTrackPointPosition + centerTrackPointDirection * 5f;
        Vector3 cameraTarget = currentPipe.GetCenterPointByProgressGlobal(progress + 0.4f);
        //Quaternion cameraRotation = Quaternion.LookRotation(cameraTarget - avatar.transform.position, GetUpVector());
        Quaternion cameraRotation = Quaternion.LookRotation(centerTrackPointDirection, GetUpVector());
        //Debug.Log(cameraRotation.eulerAngles.ToString());
        //coolVehicle.transform.rotation = cameraRotation;
        //coolVehicle.transform.rotation = Quaternion.RotateTowards(coolVehicle.transform.rotation, cameraRotation, Time.deltaTime * 100f);
        iTween.RotateUpdate(coolVehicle, iTween.Hash("rotation", cameraRotation.eulerAngles, "time", 0.1f));

        // update avatar turning according to user input
        UpdateAvatarRotation(centerTrackPointDirection, centerTrackPointPosition);
    }
    
    private Vector3 coolVehicleLookAtVelocity;
    private Vector3 coolVehicleLookAtUpVelocity;

    private void OnDrawGizmos()
    {
        // draw the point on the center track closest to the avatar
        if (Application.isPlaying)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(centerTrackPointPosition, 0.3f);
            Gizmos.DrawLine(avatar.transform.position, avatar.transform.position + centerTrackPointDirection * 6f);
            Gizmos.DrawLine(avatar.transform.position, avatar.transform.position - GetUpVector() * 6f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(centerTrackPointPosition, currentPipe.transform.TransformPoint(Vector3.zero));
            Gizmos.DrawLine(centerTrackPointPosition, avatar.transform.position);
            Gizmos.DrawLine(avatar.transform.position, currentPipe.transform.TransformPoint(Vector3.zero));

            Gizmos.color = Color.red;
            Gizmos.DrawLine(avatar.transform.position, avatar.transform.position + forceL * 4f);
            Gizmos.DrawLine(avatar.transform.position, avatar.transform.position - forceR * 4f);
        }
    }

    public Vector3 GetUpVector()
    {
        return centerTrackPointPosition - avatar.transform.position;
    }

    private void UpdateAvatarRotation(Vector3 forceDirection,Vector3 curvePoint)
    {
        float rotationInput = 0f;
        float accelerationInput = 0f;
        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 1)
            {
                Vector3 goAheadVector = avatar.transform.position + forceDirection;
                Vector3 normal = new Vector3();
                Vector3 temp = new Vector3();
                //为了让力能垂直于曲线和车 根据3点计算出左右力坐在平面的normal
                Math3d.PlaneFrom3Points(out normal,out temp, goAheadVector, curvePoint,avatar.transform.position);

                if (Input.GetTouch(0).position.x < Screen.width * 0.5f)
                {
                    avatar.GetComponent<Rigidbody>().AddForce(-normal * steeringForce, ForceMode.Acceleration);
                }
                else {
                    avatar.GetComponent<Rigidbody>().AddForce(normal * steeringForce, ForceMode.Acceleration);
                }
            }
        }
        else { // not mobile platform
            // left and right movement
            rotationInput = Input.GetAxis("Horizontal");
            Vector3 goAheadVector = avatar.transform.position + forceDirection;
            Vector3 normal = new Vector3();
            Vector3 temp = new Vector3();
            //为了让力能垂直于曲线和车 根据3点计算出左右力坐在平面的normal
            Math3d.PlaneFrom3Points(out normal, out temp, goAheadVector, curvePoint, avatar.transform.position);
            if (rotationInput > 0)
            {
                avatar.GetComponent<Rigidbody>().AddForce(-normal * steeringForce, ForceMode.Acceleration);
                forceL = -normal;
            }
            else if (rotationInput < 0)
            {
                avatar.GetComponent<Rigidbody>().AddForce(normal * steeringForce, ForceMode.Acceleration);
                forceR = normal;
            }

            // acceleration
            accelerationInput = Input.GetAxis("Vertical");
            avatar.GetComponent<Rigidbody>().AddForce(forceDirection * 5f * accelerationInput, ForceMode.Acceleration);
        }
    }
    private Vector3 forceL = Vector3.one;
    private Vector3 forceR = Vector3.one;

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
