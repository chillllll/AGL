using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public GameObject meshObj;
    public GameObject effctObj;
    public Rigidbody rigid;
    public float explodeTime; //폭발지연시간
    public float exRadius; //폭발범위
    //public int damage;
    public GameObject exColider; //폭발데미지를 줄 콜라이더
    public float destroytime;
    public AudioSource sound;

    
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
        sound.Play();
        Destroy(gameObject, destroytime);
        
    }



   
}
