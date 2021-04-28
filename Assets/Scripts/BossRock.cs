using System.Collections;
using UnityEngine;

public class BossRock : MonoBehaviour
{
    Rigidbody rigid;
    public float angularPower;
    public float plusedAngularPower; //0.02f
    public float scaleValue;
    public float plusedScaleValue; //0.005f
    public float waitTime;
    bool isShoot;


    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        StartCoroutine(GainPowerTimer());
        StartCoroutine(GainPower());
    }


    IEnumerator GainPowerTimer()
    {
        yield return new WaitForSeconds(waitTime);
        isShoot = true;
    }

    IEnumerator GainPower()
    {
        while (!isShoot)
        {
            angularPower += plusedAngularPower;
            scaleValue += plusedScaleValue;
            transform.localScale = Vector3.one * scaleValue;
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);
            yield return null;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
