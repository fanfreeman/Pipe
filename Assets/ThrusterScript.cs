using UnityEngine;
using System.Collections;

public class ThrusterScript : MonoBehaviour {
    public float thrusterStrength;
    public float thrusterDistance;
    public Transform[] thrusters;

    private Rigidbody  rigidbodyRef;

    void Awake()
    {
        rigidbodyRef = GetComponent<Rigidbody>();
    }

	void FixedUpdate ()
    {

    }

//    void OnDrawGizmos() {
//        Gizmos.color = Color.green;
//        foreach (Transform thruster in thrusters)
//        {
//            Gizmos.DrawRay(thruster.position, -(thruster.up.normalized * thrusterDistance) );
//        }
//    }
}
