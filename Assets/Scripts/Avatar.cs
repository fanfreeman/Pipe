using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {

    public ParticleSystem shape, trail, burst;

    private Player player;

    private void Awake()
    {
        player = transform.root.GetComponent<Player>();
    }
}
