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
            angularPower += plusedAngularPower; //ȸ���ӵ� ����
            scaleValue += plusedScaleValue; //ũ�� ����
            transform.localScale = Vector3.one * scaleValue; //ũ�� ������ ����
            rigid.AddTorque(transform.right * angularPower, ForceMode.Acceleration);//ȸ���ӵ� ������ ����
            yield return null;
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        if (collision.gameObject.tag == "Wall") //���� �ε��� ��
        {
            gameObject.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player") //�÷��̾ �ε��� ��
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }


}
