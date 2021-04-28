using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float bulletSpeed;
    public float destroyTime;
    // Start is called before the first frame update
    private void Awake()
    {
        Destroy(gameObject, destroyTime);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag == "floor")
        //{
        //    Destroy(gameObject, 3);
        //}

       
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }

}
