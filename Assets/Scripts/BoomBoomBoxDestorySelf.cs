using System.Collections;
using UnityEngine;

public class BoomBoomBoxDestorySelf : MonoBehaviour {

    private bool hitted = false;
    public GameObject driver;
    void Start()
    {
        StartCoroutine(WaitSomeSeconds(4f));
    }

    IEnumerator WaitSomeSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        if(hitted)return;
        hitted = true;
        Destroy(driver);
    }
}
