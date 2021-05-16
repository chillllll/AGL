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
        if (collision.gameObject.tag == "floor") //¹Ù´Ú¿¡ ´êÀ» ¶§
        {           
            gameObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Wall") //º®¿¡ ´êÀ» ¶§
        {            
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) //Àû¿¡°Ô ´êÀ» ¶§
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
