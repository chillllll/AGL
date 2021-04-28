using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public GameObject meshObj;
    public GameObject effctObj;
    public Rigidbody rigid;
    public float explodeTime;
    public float exRadius;
    //public int damage;
    public GameObject exColider;
    public float destroytime;

    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Explosion());
    }

    // Update is called once per frame
   
    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(explodeTime);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        effctObj.SetActive(true);
        exColider.SetActive(true);

        Destroy(gameObject, destroytime);
        
    }
}
