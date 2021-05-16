using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy //Enemy상속
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    public Transform rockTrans;
    public float aiDecisionTime;
    

    public AudioSource rockSound;
    public AudioSource missileSound;
    public AudioSource tauntSound;

    public ObjectPooler missilePoolA;
    public ObjectPooler missilePoolB;

    Vector3 lookVec;
    Vector3 tauntVec;
    public bool isLook;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        nav.isStopped = true;
        StartCoroutine(Think());
        
    }

    
    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            manager.clearZone.SetActive(true);
            return;
        }
        if (isLook)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            lookVec = new Vector3(h, 0, v) * 5f; //플레이어 이동방향의 조금 앞을 바라봄
            transform.LookAt(target.position + lookVec);
        }
        else
            nav.SetDestination(tauntVec); //점프공격할때 타겟셋팅
    }

    IEnumerator Think() // 랜덤행동AI
    {
        yield return new WaitForSeconds(aiDecisionTime);
        int ranAction = Random.Range(0, 5);
        switch(ranAction){
            case 0:

            case 1:
                StartCoroutine(Missileshot());
                break;
            case 2:

            case 3:
                StartCoroutine(RockShot());
                break;
            case 4:
                StartCoroutine(Taunt());
                break;
        }
    }

    IEnumerator Missileshot() //미사일
    {
        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.2f);
        missileSound.Play();
        //GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation); //미사일생성
        GameObject instantMissileA = missilePoolA.MakeObj(); //풀링한 오브젝트 활성화
        instantMissileA.transform.position = missilePortA.position;
        instantMissileA.transform.rotation = missilePortA.rotation;
        BossBullet bossBulletA = instantMissileA.GetComponent<BossBullet>();
        bossBulletA.target = target; //미사일타겟설정

        anim.SetTrigger("doShot");
        yield return new WaitForSeconds(0.3f);
        missileSound.Play();
        //GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);//미사일생성
        GameObject instantMissileB = missilePoolB.MakeObj(); //풀링한 오브젝트 활성화
        instantMissileB.transform.position = missilePortB.position;
        instantMissileB.transform.rotation = missilePortB.rotation;
        BossBullet bossBulletB = instantMissileB.GetComponent<BossBullet>();
        bossBulletB.target = target; //미사일타겟설정

        yield return new WaitForSeconds(2f);
        StartCoroutine(Think());
    }
    IEnumerator RockShot() //돌굴리기
    {
        rockSound.Play();
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bullet, rockTrans.position, transform.rotation);
        //GameObject instantBullet = objectPooler.MakeObj(); //풀링한 오브젝트 활성화
        //instantBullet.transform.position = rockTrans.position;
        //instantBullet.transform.rotation = transform.rotation;

        yield return new WaitForSeconds(3f);
        isLook = true;
        StartCoroutine(Think());
    }
    IEnumerator Taunt() //점프공격
    {
        tauntVec = target.position + lookVec; //타겟의 진행방향을 바라봄

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;

        anim.SetTrigger("doTaunt");
        tauntSound.Play();
        yield return new WaitForSeconds(1.5f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        meleeArea.enabled = false;
        
        yield return new WaitForSeconds(1f);
        isLook = true;
        nav.isStopped = true;
        boxCollider.enabled = true;
        rigid.velocity = Vector3.zero;


        StartCoroutine(Think());
    }
   

}
