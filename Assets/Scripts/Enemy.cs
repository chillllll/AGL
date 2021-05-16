using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D }; //�� ����
    public Type enemyType;
    public int maxHealth; //�ִ�ü��
    public int curHealth; //����ü��
    //public float reactforce; //�ǰݽ� �и��� ����

    public int score; //���� �� �ִ� ����
    public GameObject[] coins; //����ϴ� ������
    public int coinSize; //����ϴ� ������ ���� ����

    public Transform target; //Ÿ��(�÷��̾�)
    public GameManager manager; //���ӸŴ���
    bool isChase;
    bool isAttack;
    public bool isDead;
    public UnityEvent deathEvent; //���� �� �̺�Ʈ

    public BoxCollider meleeArea; //���� �ݶ��̴�
    public GameObject bullet;

    public float TargetDetectRadius; //�÷��̾ �����Ͽ� ������ �����ϴ� ����
    public float TargetDetectRange; 
    public float chaseDetectRadius; //�÷��̾ �����Ͽ� �i�ư��� ����
    public float chaseDetectRange;
    public float attackDelay; //���� ��������
    public float attackafterDelay; //���� �ĵ�����
    public float DamageTime; //������ �ݶ��̴� Ȱ���ð�

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    public AudioSource damagedSound;
    public AudioSource attackSound;
    public ObjectPooler objectPooler; //���Ÿ� ������ �� ��� ���Ÿ� �̻��� ������Ʈ Ǯ��


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
        
        /*if (enemyType != Type.D)
            ChaseStart();*/
    }
    
    //��������
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    
    void Update()
    {
        if (nav.enabled && enemyType != Type.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

    }
    //�÷��̾� ���������ȿ� �ִ��� ����
    void Detecting()
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = chaseDetectRadius; //����ĳ��Ʈ ������
            float targetRange = chaseDetectRange;

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
                targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player")); //��ü�� ����ĳ��Ʈ�� �÷��̾� ���̾� ����

            if (rayHits.Length > 0 && !isChase && !isAttack) //����ĳ��Ʈ�� �÷��̾� �����
            {

                ChaseStart();
            }
        }
    }
    //�÷��̾� ���ݹ����ȿ� �ִ��� ����
    void Targeting()
    {
        if (!isDead&& enemyType != Type.D)
        {
            float detectRadius = TargetDetectRadius; //����ĳ��Ʈ ������
            float detectRange = TargetDetectRange;

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
                detectRadius, transform.forward, detectRange, LayerMask.GetMask("Player")); //��ü�� ����ĳ��Ʈ�� �÷��̾� ���̾� ����
            
            if (rayHits.Length > 0 && !isAttack) //����ĳ��Ʈ�� �÷��̾� �����
            {
                isChase = false;
                StartCoroutine(Attack()); //���� �ڷ�ƾ ����
                
            }
        }
    }
    

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        //������ �� ���������� Ȱ��ȭ

        switch (enemyType) //Ÿ�Ժ� ��������
        {
            case Type.A: //���� ����
                attackSound.Play();
                yield return new WaitForSeconds(attackDelay);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(DamageTime);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(attackafterDelay);
                break;

            case Type.B: //��� ����
                isAttack = false;
                attackSound.Play();
                yield return new WaitForSeconds(attackDelay);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;
                isAttack = true;
                rigid.velocity = Vector3.zero;

                yield return new WaitForSeconds(DamageTime);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(attackafterDelay);
                
                break;

            case Type.C: //�̻��� �߻�
                
                yield return new WaitForSeconds(attackDelay);
                //GameObject instantBullet = objectPooler.GetObj(); 
                GameObject instantBullet = objectPooler.MakeObj(); //Ǯ���� ������Ʈ Ȱ��ȭ
                instantBullet.transform.position = transform.position;
                instantBullet.transform.rotation = transform.rotation;
                //GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
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
    void FreezeVelocity() //�и� ����
    {
        if (isChase||isAttack)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        Detecting();
        Targeting();
        FreezeVelocity();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee") //�������ݿ� �ǰݽ�
        {
            Weapon weapon = other.GetComponent<Weapon>(); //���� ���� ����
            curHealth -= weapon.damage; //���� ü�� ����
            //Vector3 reactVec = transform.position - other.transform.position; //���� ������ �̴� ���� ������
           // reactforce = weapon.KnockbackForce; //���� ������ �̴� ��=���� �и��� ��
            StartCoroutine(OnDamage()); //������ ���� ���� �ڷ�ƾ ����
        }
        else if (other.tag == "Bullet") //���Ÿ����ݿ� �ǰݽ�
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
           // Vector3 reactVec = transform.position - other.transform.position;
           // reactforce = bullet.KnockbackForce;
            StartCoroutine(OnDamage());
        }
        else if (other.tag == "DamageZone") //���߿� �ǰݽ�
        {
            ExplosionDamage explosionDamage = other.GetComponent<ExplosionDamage>();
            curHealth -= explosionDamage.damage;
            //Vector3 reactVec = transform.position - other.transform.position;
           // reactforce = explosionDamage.KnockbackForce;
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage() //�ǰݽ�
    {
        damagedSound.Play();
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; //�ٵ� ���������� �ٲ�

        /* //�ǰ� �� �з���
            reactVec = reactVec.normalized;
            float reactVecX = reactVec.x;
            float reactVecZ = reactVec.z;
            float hitbackX = reactVecX * reactforce;
            float hitbackZ = reactVecZ * reactforce;
            
            rigid.AddForce(hitbackX,0,hitbackZ, ForceMode.Impulse); //�ǰ� �ݴ�������� reactforce��ŭ �з���
        */

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //�ٵ� �ٽ� ���� ������ �ٲ�
        }
        else //ü���� 0������ ��
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;  //�ٵ� ȸ������ �ٲ�
            gameObject.layer = 12; //��ü ���̾ enemyDead���̾�� �ٲ�
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie"); //�״� �ִϸ��̼� ����
            Player player = target.GetComponent<Player>();
            player.score += score; //�÷��̾� ���ھ� ����
            int rancoin = Random.Range(0, coinSize);
            Instantiate(coins[rancoin], transform.position, Quaternion.identity); //Quaternion.identity  ->ȸ������
            //������ ���� 10�� �Ѹ���
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

            switch (enemyType) //���ӸŴ������� �� ī��Ʈ ������
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
            
            if (deathEvent != null)
                deathEvent.Invoke();

            yield return new WaitForSeconds(4f);
            gameObject.SetActive(false); //4���� ��Ȱ��ȭ
        }
    }


}
