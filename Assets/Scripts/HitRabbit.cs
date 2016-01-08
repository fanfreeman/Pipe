using UnityEngine;
using System.Collections;

public class HitRabbit : MonoBehaviour {
    public BoxCollider triggerCollier;
 //   public SphereCollider pushMeToSkyCollider;

    void OnTriggerEnter(Collider collider)
    {
        GetComponent<AudioSource>().Play();

        //撞击效果
        Player player = collider.transform.parent.gameObject.GetComponent<Player>();
        if(player != null)
        {
            player.BoomByRabbitEffect();
        }
    }

    void OnCollisionExit()
    {
  //      pushMeToSkyCollider.enabled = false;
    }
}
