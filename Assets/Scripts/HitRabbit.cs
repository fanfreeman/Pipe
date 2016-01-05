using UnityEngine;
using System.Collections;

public class HitRabbit : MonoBehaviour {
    public BoxCollider triggerCollier;
    public SphereCollider pushMeToSkyCollider;

    void OnTriggerEnter()
    {
        Debug.Log("打兔子");
        GetComponent<AudioSource>().Play();
        pushMeToSkyCollider.enabled = true;
    }

    void OnCollisionExit()
    {
        pushMeToSkyCollider.enabled = false;
    }
}
