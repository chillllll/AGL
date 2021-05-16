using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{

    public GameObject meshObj;
    public GameObject effctObj;
    public Rigidbody rigid;
    public float explodeTime; //���������ð�
    public float exRadius; //���߹���
    //public int damage;
    public GameObject exColider; //���ߵ������� �� �ݶ��̴�
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
