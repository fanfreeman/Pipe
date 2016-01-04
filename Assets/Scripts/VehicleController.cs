using UnityEngine;
using System.Collections;

public class VehicleController : MonoBehaviour {

    [SerializeField]
    private VehicleMouseLook m_MouseLook;

    private Camera m_Camera;
    private Vector3 m_OriginalCameraPosition;

    void Start () {
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_MouseLook.Init(transform, m_Camera.transform);
    }
	
	void Update () {
        RotateView();
    }

    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }
}
