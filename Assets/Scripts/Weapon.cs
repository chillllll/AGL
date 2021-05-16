using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range }; //무기 타입(근거리,원거리)
    public Type type;
    public int damage;
    public float AttackSpeed; //공격속도
    public float bulletSpeed; //총알 속도
    public int maxAmmo; //최대탄창수
    public int curAmmo; //현재탄창수
    public float reloadTime = 1f; //재장전시간
    public float KnockbackForce; //타격시 적을 미는 힘

    public BoxCollider meleeArea; //타격범위 콜라이더
    public TrailRenderer trailEffect;
    public Transform bulletPosition;
    //public GameObject bullet; //총알 프리펩
    public Transform bulletCasePosition;
    //public GameObject bulletCase;
    public AudioSource attackSound;

    public ObjectPooler objectPooler;
    public ObjectPooler objectPoolerBulletCase;

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
        //GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        GameObject instantBullet = objectPooler.MakeObj();
        //GameObject instantBullet = objectPooler.GetObj();
        instantBullet.transform.position = bulletPosition.position;
        instantBullet.transform.rotation = bulletPosition.rotation;
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        

        bulletRigid.velocity = bulletPosition.forward * bulletSpeed;
        yield return null;
        //2. 탄피 배출
        //GameObject instantCase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
        GameObject instantCase = objectPoolerBulletCase.MakeObj();
        //GameObject instantCase = objectPooler.GetObj();
        instantCase.transform.position = bulletCasePosition.position;
        instantCase.transform.rotation = bulletCasePosition.rotation;
        Rigidbody bulletCaseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePosition.forward * Random.Range(2, 3) + Vector3.up;
        bulletCaseRigid.AddForce(caseVec, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);

    }


}
