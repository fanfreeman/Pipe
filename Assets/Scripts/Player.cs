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
        currentPipe.GetPlaneOfCurve(
                avatar.transform.TransformPoint(avatar.transform.position),
                out centerTrackPointDirection,
                out centerTrackPointPosition
        );
    }

    private void 找到正确的圆心()
    {

    }



    private void Update()
    {
        getUpVectorHolder = GetUpVector();

        currentPipe.GetPlaneOfCurve(
                avatar.transform.position,
                out centerTrackPointDirection,
                out centerTrackPointPosition
        );

//        while (newDistanceToAvatar < distanceToAvatar)
//        {
//            // move track hook by setting a new value for progress along the spline
//            progress += progressDelta;
//
//            // check if we have traveled the entire length of a pipe segment
//            // and should set up the next pipe
//            if (progress > 1f)
//            {
//                prevPipe = currentPipe;
//                currentPipe = pipeSystem.SetupNextPipe();
//                progress -= 1f;
//            }
//
//            centerTrackPointPosition = currentPipe.cameraSpline.GetPoint(progress);
//            distanceToAvatar = newDistanceToAvatar;
//            newDistanceToAvatar = Vector3.Distance(currentPipe.cameraSpline.GetPoint(progress + progressDelta), avatar.transform.position);
//        }

        // apply force to move forward
   //     centerTrackPointDirection = currentPipe.cameraSpline.GetVelocity(progress);
        avatar.GetComponent<Rigidbody>().AddForce(centerTrackPointDirection * 5f, ForceMode.Acceleration);

        // apply force to make avatar stick to wall
        Vector3 upVector = GetUpVector();
        Debug.Log(upVector.magnitude);
        float magnitudeModifier = (3f - upVector.magnitude) * 10f;
        avatar.GetComponent<Rigidbody>().AddForce(-upVector * magnitudeModifier, ForceMode.Acceleration);

        Vector3 lookAt = Vector3.SmoothDamp(coolVehicle.transform.position, avatar.transform.position, ref coolVehicleLookAtVelocity, 0.05f);

        Vector3 forwardVector =  avatar.transform.position - coolVehicle.transform.position;
        var newRot = Quaternion.LookRotation(forwardVector ,upVector);
        coolVehicle.transform.rotation = Quaternion.Lerp(coolVehicle.transform.rotation, newRot,0.5f);

        coolVehicle.transform.position = lookAt;
        //UpdateAvatarRotation();
        //hud.SetValues(distanceTraveled, velocity);
        Vector3 forceDirection = centerTrackPointDirection;
        UpdateAvatarRotation(forceDirection, centerTrackPointPosition);
    }

    private Vector3 getUpVectorHolder;
    private Vector3 coolVehicleLookAtVelocity;
    private Vector3 coolVehicleLookAtUpVelocity;

    private void OnDrawGizmos()
    {
        // draw the point on the center track closest to the avatar
        Gizmos.DrawSphere(centerTrackPointPosition, 0.5f);
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
            if(rotationInput != 0)
            {
                Vector3 goAheadVector = avatar.transform.position + forceDirection;
                Vector3 normal = new Vector3();
                Vector3 temp = new Vector3();
                //为了让力能垂直于曲线和车 根据3点计算出左右力坐在平面的normal
                Math3d.PlaneFrom3Points(out normal,out temp, goAheadVector, curvePoint,avatar.transform.position);
                if( rotationInput > 0 )
                {
                    avatar.GetComponent<Rigidbody>().AddForce(-normal * 12f, ForceMode.Acceleration);
                }
                else if(rotationInput < 0)
                {
                    avatar.GetComponent<Rigidbody>().AddForce(normal * 12f, ForceMode.Acceleration);
                }
            }
        }
        Debug.Log("rotationInput："+rotationInput);
    }

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
