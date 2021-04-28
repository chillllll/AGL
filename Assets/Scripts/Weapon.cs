using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range };
    public Type type;
    public int damage;
    public float AttackSpeed;
    public float bulletSpeed;
    public int maxAmmo;
    public int curAmmo;
    public float reloadTime = 1f;
    public float KnockbackForce;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;
    public Transform bulletPosition;
    public GameObject bullet;
    public Transform bulletCasePosition;
    public GameObject bulletCase;
    public AudioSource attackSound;

    public void Use()
    {
        if (type == Type.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }

        else if (type == Type.Range || curAmmo > 0)
        {
            StopCoroutine("Shot");
            curAmmo--;
            StartCoroutine("Shot");
        }
    }

    IEnumerator Swing()
    {
        //1.데미지,이펙트 활성화
        yield return new WaitForSeconds(0.1f);
        attackSound.Play();
        meleeArea.enabled = true;
        trailEffect.enabled = true;
        //2.비활성화
        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.5f);
        trailEffect.enabled = false;

        yield break;
    }

    IEnumerator Shot()
    {
        //1. 총알 발사
        attackSound.Play();
        GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();

        bulletRigid.velocity = bulletPosition.forward * bulletSpeed;
        yield return null;
        //2. 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
        Rigidbody bulletCaseRigid = instantBullet.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePosition.forward * Random.Range(2, 3) + Vector3.up;
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }


}
