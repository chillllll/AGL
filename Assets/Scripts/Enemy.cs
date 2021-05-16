using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Enemy : MonoBehaviour
{
    public enum Type { A, B, C, D }; //적 종류
    public Type enemyType;
    public int maxHealth; //최대체력
    public int curHealth; //현재체력
    //public float reactforce; //피격시 밀리는 정도

    public int score; //잡을 시 주는 점수
    public GameObject[] coins; //드랍하는 아이템
    public int coinSize; //드랍하는 아이템 종류 갯수

    public Transform target; //타겟(플레이어)
    public GameManager manager; //게임매니저
    bool isChase;
    bool isAttack;
    public bool isDead;
    public UnityEvent deathEvent; //죽을 때 이벤트

    public BoxCollider meleeArea; //공격 콜라이더
    public GameObject bullet;

    public float TargetDetectRadius; //플레이어를 감지하여 공격을 수행하는 범위
    public float TargetDetectRange; 
    public float chaseDetectRadius; //플레이어를 감지하여 쫒아가는 범위
    public float chaseDetectRange;
    public float attackDelay; //공격 선딜레이
    public float attackafterDelay; //공격 후딜레이
    public float DamageTime; //데미지 콜라이더 활성시간

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshs;
    public NavMeshAgent nav;
    public Animator anim;

    public AudioSource damagedSound;
    public AudioSource attackSound;
    public ObjectPooler objectPooler; //원거리 공격을 할 경우 원거리 미사일 오브젝트 풀링


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
    
    //추적시작
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
    //플레이어 추적범위안에 있는지 감지
    void Detecting()
    {
        if (!isDead && enemyType != Type.D)
        {
            float targetRadius = chaseDetectRadius; //레이캐스트 반지름
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
                targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player")); //구체형 레이캐스트로 플레이어 레이어 검출

            if (rayHits.Length > 0 && !isChase && !isAttack) //레이캐스트에 플레이어 검출시
            {

                ChaseStart();
            }
        }
    }
    //플레이어 공격범위안에 있는지 감지
    void Targeting()
    {
        if (!isDead&& enemyType != Type.D)
        {
            float detectRadius = TargetDetectRadius; //레이캐스트 반지름
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
                detectRadius, transform.forward, detectRange, LayerMask.GetMask("Player")); //구체형 레이캐스트로 플레이어 레이어 검출
            
            if (rayHits.Length > 0 && !isAttack) //레이캐스트에 플레이어 검출시
            {
                isChase = false;
                StartCoroutine(Attack()); //공격 코루틴 실행
                
            }
        }
    }
    

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
        //딜레이 후 데미지영역 활성화

        switch (enemyType) //타입별 공격패턴
        {
            case Type.A: //근접 공격
                attackSound.Play();
                yield return new WaitForSeconds(attackDelay);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(DamageTime);
                meleeArea.enabled = false;
                yield return new WaitForSeconds(attackafterDelay);
                break;

            case Type.B: //대시 공격
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

            case Type.C: //미사일 발사
                
                yield return new WaitForSeconds(attackDelay);
                //GameObject instantBullet = objectPooler.GetObj(); 
                GameObject instantBullet = objectPooler.MakeObj(); //풀링한 오브젝트 활성화
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
    void FreezeVelocity() //밀림 제어
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
        if (other.tag == "Melee") //근접공격에 피격시
        {
            Weapon weapon = other.GetComponent<Weapon>(); //맞은 무기 검출
            curHealth -= weapon.damage; //현재 체력 감소
            //Vector3 reactVec = transform.position - other.transform.position; //맞은 무기의 미는 힘을 가져옴
           // reactforce = weapon.KnockbackForce; //맞은 무기의 미는 힘=적이 밀리는 힘
            StartCoroutine(OnDamage()); //데미지 입을 시의 코루틴 실행
        }
        else if (other.tag == "Bullet") //원거리공격에 피격시
        {
            Bullet bullet = other.GetComponent<Bullet>();
            curHealth -= bullet.damage;
           // Vector3 reactVec = transform.position - other.transform.position;
           // reactforce = bullet.KnockbackForce;
            StartCoroutine(OnDamage());
        }
        else if (other.tag == "DamageZone") //폭발에 피격시
        {
            ExplosionDamage explosionDamage = other.GetComponent<ExplosionDamage>();
            curHealth -= explosionDamage.damage;
            //Vector3 reactVec = transform.position - other.transform.position;
           // reactforce = explosionDamage.KnockbackForce;
            StartCoroutine(OnDamage());
        }
    }

    IEnumerator OnDamage() //피격시
    {
        damagedSound.Play();
        foreach (MeshRenderer mesh in meshs)
            mesh.material.color = Color.red; //바디를 빨간색으로 바꿈

        /* //피격 시 밀려남
            reactVec = reactVec.normalized;
            float reactVecX = reactVec.x;
            float reactVecZ = reactVec.z;
            float hitbackX = reactVecX * reactforce;
            float hitbackZ = reactVecZ * reactforce;
            
            rigid.AddForce(hitbackX,0,hitbackZ, ForceMode.Impulse); //피격 반대방향으로 reactforce만큼 밀려남
        */

        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0)
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.white; //바디를 다시 원래 색으로 바꿈
        }
        else //체력이 0이하일 때
        {
            foreach (MeshRenderer mesh in meshs)
                mesh.material.color = Color.gray;  //바디를 회색으로 바꿈
            gameObject.layer = 12; //개체 레이어를 enemyDead레이어로 바꿈
            isDead = true;
            isChase = false;
            nav.enabled = false;
            anim.SetTrigger("doDie"); //죽는 애니메이션 수행
            Player player = target.GetComponent<Player>();
            player.score += score; //플레이어 스코어 증가
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

            switch (enemyType) //게임매니저에서 적 카운트 내리기
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
            gameObject.SetActive(false); //4초후 비활성화
        }
    }


}
