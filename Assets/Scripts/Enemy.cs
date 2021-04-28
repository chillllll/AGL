using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D };
    public Type enemyType;
    public int maxHealth;
    public int curHealth;
    public float reactforce;

    public int score;
    public GameObject[] coins;
    public int coinSize;

    public Transform target;
    public GameManager manager;
    public bool isChase;
    public bool isAttack;
    public bool isDead;
    public UnityEvent deathEvent;

    public BoxCollider meleeArea;
    public GameObject bullet;

    public float TargetDetectRadius;
    public float TargetDetectRange;
    public float attackDelay;
    public float attackafterDelay;
    public float DamageTime;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;
    public AudioSource damagedSound;
    public AudioSource attackSound;


    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        nav.enabled = false;
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        nav.enabled = true;
        anim = GetComponentInChildren<Animator>();
        if (enemyType != Type.D)
            ChaseStart();
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    // Update is called once per frame
    void Update()
    {
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

    }
    void Targeting()
    {
        if (!isDead&& enemyType != Type.D)
        {
            float targetRadius = TargetDetectRadius;
            float targetRange = TargetDetectRange;

            //switch (enemyType)
            //{
            //    case Type.A:
            //        break;
            //    case Type.B:
            //        break;
            //    case Type.C:
            //        break;
            //}

            RaycastHit[] rayHits = Physics.SphereCastAll(transform.position,
                targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

            if (rayHits.Length > 0 && !isAttack)
            {

                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        //딜레이 후 데미지영역 활성화

        switch (enemyType)
        {
            case Type.A:
                attackSound.Play();
                yield return new WaitForSeconds(attackDelay);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(DamageTime);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(attackafterDelay);
                break;

            case Type.B:
                attackSound.Play();
                yield return new WaitForSeconds(attackDelay);
                rigid.AddForce(transform.forward * 10, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(DamageTime);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(attackafterDelay);
                break;

            case Type.C:
                
                yield return new WaitForSeconds(attackDelay);
                GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
                Rigidbody rigbullet = instantBullet.GetComponent<Rigidbody>();
                Missile bulletSpeed = instantBullet.GetComponent<Missile>();
                rigbullet.velocity = transform.forward * bulletSpeed.bulletSpeed;
                attackSound.Play();
                yield return new WaitForSeconds(DamageTime);

                yield return new WaitForSeconds(attackafterDelay);
                break;

        }
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);

    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            curHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            reactforce = weapon.KnockbackForce;
            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            reactforce = bullet.KnockbackForce;
            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "DamageZone")
        {
            ExplosionDamage explosionDamage = other.GetComponent<ExplosionDamage>();
            curHealth -= explosionDamage.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            reactforce = explosionDamage.KnockbackForce;
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        damagedSound.Play();
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white;
        }
        else //체력이 0이하일 때
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;
            gameObject.layer = 12;
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie");
            Player player = target.GetComponent<Player>();
            player.score += score;
            int rancoin = Random.Range(0, coinSize);
            Instantiate(coins[rancoin], transform.position, Quaternion.identity); //Quaternion.identity  ->회전없음
            //보스면 코인 10개 뿌리기
            if (enemyType == Type.D) { 
                for(int i = 0; i > 5; i++) {
                    Vector3 ranvec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3);
                    Instantiate(coins[rancoin], transform.position, Quaternion.identity);
                }
                for (int i = 0; i > 5; i++)
                {
                    Vector3 ranvec = Vector3.left * Random.Range(-3, 3) + Vector3.back * Random.Range(-3, 3);
                    Instantiate(coins[rancoin], transform.position, Quaternion.identity);
                }
            }

            switch (enemyType)
            {
                case Type.A:
                    manager.enemyCntA--;
                    break;
                case Type.B:
                    manager.enemyCntB--;
                    break;
                case Type.C:
                    manager.enemyCntC--;
                    break;
            }
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * reactforce, ForceMode.Impulse);
            if (deathEvent != null)
                deathEvent.Invoke();
            
            Destroy(gameObject, 4);
        }
    }


}
