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

        if(progress >= 1)
            currentPipe = pipeSystem.SetupNextPipe();


        // apply force to move forward
        avatar.GetComponent<Rigidbody>().AddForce(centerTrackPointDirection * 15f, ForceMode.Acceleration);

        // apply force to make avatar stick to wall
        Vector3 upVector = GetUpVector();
        float magnitudeModifier = (3f - upVector.magnitude) * 10f;
        avatar.GetComponent<Rigidbody>().AddForce(-upVector * magnitudeModifier, ForceMode.Acceleration);

        //Vector3 lookAt = avatar.transform.position;// Vector3.SmoothDamp(coolVehicle.transform.position, avatar.transform.position, ref coolVehicleLookAtVelocity, 0.03f);

        //Vector3 forwardVector =  avatar.transform.position - coolVehicle.transform.position;
        //var newRot = Quaternion.LookRotation(forwardVector ,upVector);
        //   coolVehicle.transform.rotation = Quaternion.Lerp(coolVehicle.transform.rotation, newRot, );


        //UpdateAvatarRotation();
        //hud.SetValues(distanceTraveled, velocity);

        // update camera position
        coolVehicle.transform.position = avatar.transform.position;

        // update camera rotation
        Quaternion cameraRotation = Quaternion.LookRotation(centerTrackPointPosition + centerTrackPointDirection * 5f - avatar.transform.position, GetUpVector());
        Debug.Log(cameraRotation.eulerAngles.ToString());
        //coolVehicle.transform.rotation = cameraRotation;
        coolVehicle.transform.rotation = Quaternion.RotateTowards(coolVehicle.transform.rotation, cameraRotation, Time.deltaTime * 100f);

        // update avatar turning according to user input
        UpdateAvatarRotation(centerTrackPointDirection, centerTrackPointPosition);
    }
    
    private Vector3 coolVehicleLookAtVelocity;
    private Vector3 coolVehicleLookAtUpVelocity;

    private void OnDrawGizmos()
    {
<<<<<<< HEAD
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
=======
        // draw the center track point
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(centerTrackPointPosition, 0.3f);

        // draw the center track direction
        Gizmos.DrawLine(centerTrackPointPosition, centerTrackPointPosition + centerTrackPointDirection * 5f);

        //Gizmos.DrawLine(avatar.transform.position, avatar.transform.position + centerTrackPointDirection * 6f);
        //Gizmos.DrawLine(avatar.transform.position, avatar.transform.position - GetUpVector() * 6f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(avatar.transform.position, avatar.transform.position + forceL * 4f);
        Gizmos.DrawLine(avatar.transform.position, avatar.transform.position - forceR * 4f);
>>>>>>> 3a6c9143a58b18c87b824ff9bf7ec7bb1d41c214
    }

    public Vector3 GetUpVector()
    {
        return centerTrackPointPosition - avatar.transform.position;
    }

    private void UpdateAvatarRotation(Vector3 forceDirection,Vector3 curvePoint)
    {
        float rotationInput = 0f;
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
                    avatar.GetComponent<Rigidbody>().AddForce(-normal * 12f, ForceMode.Acceleration);
                }
                else {
                    avatar.GetComponent<Rigidbody>().AddForce(normal * 12f, ForceMode.Acceleration);
                }
            }
        }
        else {
            rotationInput = Input.GetAxis("Horizontal");
            if(true)
            {
                Vector3 goAheadVector = avatar.transform.position + forceDirection;
                Vector3 normal = new Vector3();
                Vector3 temp = new Vector3();
                //为了让力能垂直于曲线和车 根据3点计算出左右力坐在平面的normal
                Math3d.PlaneFrom3Points(out normal,out temp, goAheadVector, curvePoint,avatar.transform.position);
                if( rotationInput > 0 )
                {
                    avatar.GetComponent<Rigidbody>().AddForce(-normal * 12f, ForceMode.Acceleration);
                    forceL = - normal;
                }
                else if(rotationInput < 0)
                {
                    avatar.GetComponent<Rigidbody>().AddForce(normal * 12f, ForceMode.Acceleration);
                    forceR = normal;
                }
            }
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
