using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public float KnockbackForce;
    public float destroyTime;
    

    private void Awake()
    {
       
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor") //�ٴڿ� ���� ��
        {           
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Wall") //���� ���� ��
        {            
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) //������ ���� ��
    {
        if (other.gameObject.tag == "Enemy")
        {
            Invoke("deActive", 0.05f);
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
