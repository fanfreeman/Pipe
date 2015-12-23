using UnityEngine;
using System.Collections;

public class CarController : MonoBehaviour {
	public float acceleration;
    public float rotationRate;

    public float turnRotationAngle;
    public float turnRotationSeekSpeed;

    private float rotationVelocity;
    private float groundAngleVelocity;
    private Rigidbody rigidbody;

    void Awake(){
        rigidbody = GetComponent<Rigidbody>();
    }

	void FixedUpdate()
    {
            Vector3 forwardForce = transform.forward * acceleration * Input.GetAxis("Vertical");
         //   forwardForce = forwardForce * Time.deltaTime * rigidbody.mass;
            rigidbody.AddForce(forwardForce * 0.01f, ForceMode.Acceleration);
//        Vector3 turnTorque = Vector3.up * rotationRate * Input.GetAxis("Horizontal");
//        turnTorque *= Time.deltaTime * rigidbody.mass;
//        rigidbody.AddTorque(turnTorque);
//
//        Vector3 newRotation = transform.eulerAngles;
//        newRotation.z = Mathf.SmoothDampAngle(newRotation.z, Input.GetAxis("Horizontal") * -turnRotationAngle, ref rotationVelocity, turnRotationSeekSpeed);
//        transform.eulerAngles = newRotation;
    }
}
