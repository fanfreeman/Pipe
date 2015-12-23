using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    public Player player;

    void Awake()
    {
        player = GetComponentInParent<Player>();
    }

	// Update is called once per frame
	void Update () {
        transform.LookAt(player.transform);
	}
}
