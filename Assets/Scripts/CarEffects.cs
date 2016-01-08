using UnityEngine;
using System.Collections;

public class CarEffects : MonoBehaviour {
	private Transform car;

    //rotation
    private bool isRotating = false;
    private float rotationTime;
    private float rotationSpeed;
    private float rotationDelta;

	void Awake () {
		car = transform.FindChild("car");
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        //rotation driver
        if(isRotating)
        {
            rotationTime -= Time.fixedDeltaTime;
            car.Rotate(
                    Vector3.forward * rotationSpeed * Time.fixedDeltaTime
            );
            //让车转回原位
            if(rotationTime <= 0)
            {
                if((Mathf.Abs(car.rotation.z)%360) > rotationDelta)
                {
                    car.Rotate(
                            Vector3.forward * rotationSpeed * Time.fixedDeltaTime
                    );
                }else
                {
                    car.localRotation = Quaternion.Euler(
                            Vector3.zero
                    );
                    isRotating = false;
                }
            }
        }


	}

    //让车旋转
    public void CarRotate(int speed, float time){
        isRotating = true;
        rotationTime = time;
        rotationSpeed = speed;
        //0.03448275862069 1/29
        rotationDelta = 360 * 0.03448275862069f;
    }
}
