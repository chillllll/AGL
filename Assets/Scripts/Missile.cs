using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public float bulletSpeed;
    public float destroyTime;
    
    void Update()
    {
        //transform.Rotate(Vector3.right * 30 * Time.deltaTime);
    }
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") //�÷��̾�� �ε�����
        {
            gameObject.SetActive(false);
        }
        if (other.gameObject.tag == "Wall") //���� �ε�����
        {
            gameObject.SetActive(false);
        }
    }

    void deActive()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Invoke("deActive", destroyTime);

    }
    private void OnDisable()
    {
        CancelInvoke();
    }

}
