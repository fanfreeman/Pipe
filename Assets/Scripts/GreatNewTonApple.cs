using UnityEngine;
using System.Collections;

public class GreatNewTonApple : MonoBehaviour {
    private Rigidbody rig;
    private Collider collider1;

    public GameObject boomObject;
    private void OnTriggerEnter(Collider collider)
    {
        //��Ӹ����colliderȻ����ײ��
        rig = gameObject.AddComponent<Rigidbody>();
        rig.mass = 1;
        rig.useGravity = false;

        collider1 = gameObject.AddComponent<BoxCollider>();
        collider1.isTrigger = false;
        if(true)
        {
            Instantiate(boomObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(DestroyUselessComponent());
        }
    }

    IEnumerator DestroyUselessComponent()
    {
        yield return new WaitForSeconds(4f);
        Destroy(rig);
        Destroy(collider1);
    }
}
