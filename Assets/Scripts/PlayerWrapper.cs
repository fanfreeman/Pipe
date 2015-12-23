using UnityEngine;
using System.Collections;

public class PlayerWrapper : MonoBehaviour {

    private Player player;

	// Use this for initialization
	void Awake () {
        player = GetComponentInChildren<Player>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(player.GetCenterTrackHookPosition(), Vector3.up);
    }
}
