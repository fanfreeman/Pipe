using UnityEngine;
using System.Collections;

public class SetupCameraCollider : MonoBehaviour {
	private Camera camera;

    private Transform tr;
    private Transform br;
    private Transform tl;
    private Transform bl;

    private const float RESET_TIME = 10f;
    private float timeDelta = RESET_TIME;

    // Use this for initialization
	void Awake () {
        tr = transform.FindChild("tr");
        br = transform.FindChild("br");
        tl = transform.FindChild("tl");
        bl = transform.FindChild("bl");
        camera = GetComponent<Camera>();
    }

    void Start()
    {

    }

    //将4个collider map到屏幕四角
    private void SetColliderToFitViewPoint()
    {
        tr.transform.position = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
        br.transform.position = camera.ViewportToWorldPoint(new Vector3(1, 0, camera.nearClipPlane));
        tl.transform.position = camera.ViewportToWorldPoint(new Vector3(0, 1, camera.nearClipPlane));
        bl.transform.position = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
    }
	

	void FixedUpdate () {
        timeDelta -= Time.deltaTime;
        //每隔 10f修正一下collider位置
        if(timeDelta < 0)
        {
            timeDelta = RESET_TIME;
            SetColliderToFitViewPoint();
        }
	}
}
