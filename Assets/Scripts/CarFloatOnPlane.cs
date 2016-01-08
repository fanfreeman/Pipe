using UnityEngine;
using System.Collections;

public class CarFloatOnPlane : MonoBehaviour {
    public Transform[] thrusters;
    private float thrusterDistance = 15f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update ()
    {
        float dX;
        float dZ;
        float[] thrusDistance = new float[4];

        RaycastHit hit;
        for (int i = 0; i < 4; i++)
        {
            Transform thruster = thrusters[i];
            if (Physics.Raycast (thruster.position, thruster.up * -1, out hit, thrusterDistance,1<<8))
            {
                thrusDistance[i] = hit.distance;
            }
        }
        dX = thrusDistance[0] - thrusDistance[1];
        dZ = thrusDistance[2] - thrusDistance[3];
    }
}
