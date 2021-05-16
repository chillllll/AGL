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
            angularPower += plusedAngularPower; //회전속도 증가
            scaleValue += plusedScaleValue; //크기 증가
            transform.localScale = Vector3.one * scaleValue; //크기 증가값 적용
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);//회전속도 증가값 적용
            yield return null;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        if (collision.gameObject.tag == "Wall") //벽에 부딪힐 때
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") //플레이어에 부딪힐 때
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
