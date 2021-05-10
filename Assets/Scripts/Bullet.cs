using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    public int damage;
    public float KnockbackForce;
    public float destroyTime;


    private void Awake()
    {
        Destroy(gameObject, destroyTime);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor") //¹Ù´Ú¿¡ ´êÀ» ¶§
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Wall") //º®¿¡ ´êÀ» ¶§
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) //Àû¿¡°Ô ´êÀ» ¶§
    {
        if (other.gameObject.tag == "Enemy")
        {
            Destroy(gameObject,0.05f);
        }
    }
}
