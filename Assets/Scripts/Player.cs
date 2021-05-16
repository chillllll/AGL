using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    public float speed; //이동속도
    public VariableJoystick variableJoystick;
    public GameObject iButton; //상호작용버튼
    public float jumpHeight; 
    public GameObject[] weapons; //무기들
    public bool[] hasWeapon; //무기 보유 여부
    //public GameObject[] grenade;
    public int hasGrenades; //가진 수류탄 갯수
    public ObjectPooler grenadePool;
    public GameObject GrenadeObj;
    public int sateliteQuantity;
    public Transform faceDirection; //보는 방향
    public GameManager manager; //게임매니저

    public int ammo; //총알
    public int coin; //코인
    public int health; //체력
    public int score; //점수

    public int Maxammo; //최대총알갯수
    public int Maxcoin; //최대코인
    public int Maxhealth; //최대체력
    public int MaxhasGrenades; //최대수류탄갯수

    public float invulnerableTime; //무적시간

    float hAxis;
    float vAxis;

    Vector3 moveVec; //이동방향
    Vector3 dodgeVec; //회피방향
    public bool wDown; //이동키누름
    //public bool jDown;  
    public bool dDown; //회피키누름
    public AudioSource dodgeSound;
    public bool iDown; //상호작용키누름
    public AudioSource interSound;
    public bool zDown; //공격키누름
    public bool rDown; //장전키누름
    public AudioSource reloadSound;
    public bool gDown; //수류탄키누름
    public AudioSource grenadeSound;
    public bool sDown1; //무기스왑키1누름
    public bool sDown2; //무기스왑키2누름
    public bool sDown3; //무기스왑키3누름
    public AudioSource swapSound;
    public AudioSource damagedSound;
    public AudioSource coinSound;

    //플레이어 행동 상태
    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isReload;
    bool isAttackReady = true;
    public bool isBorder;
    bool isDamaged;
    bool isShop;
    bool isBossAtk;
    bool isDead;

    Animator anim;
    Rigidbody rigid;
    MeshRenderer[] meshs;

    GameObject nearObject;
    public Weapon equipWeapon;
    int equipWeaponIndex = -1;
    float AttackDelay;

    //가까운적 오토타겟팅을 위한 변수
    Transform shortTargetTrans; //플레이어
    [Tooltip("타겟팅 범위")]
    public float targetRange;
    [Tooltip("타겟 레이어")]
    [SerializeField] LayerMask targetLayer;
    [Tooltip("타겟 트랜스폼")]
    Transform targetTransform=null;


    public Vector3 hitVec;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        meshs = GetComponentsInChildren<MeshRenderer>();
        InvokeRepeating("SearchEnemy", 0f, 0.5f);
        //PlayerPrefs.SetInt("MaxScore", 0);
        //Debug.Log(PlayerPrefs.GetInt("MaxScore"));
        
    }
    //회전방지
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    //벽에서 비비지 않고 멈춤
    void StopToWall() //벽에서 비비지않고 멈춤
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall")); //레이를 쏴서 벽에 가까울시 멈춤
    }
    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void Update()
    {
        Attack();
        //getInput();
        Move();
        Turn();
        //Jump();
        Dodge();
        Interaction();
        Swap();
        Reload();
        Grenade();
       
    }

    //인풋받기-모바일 사용안함
    void getInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        //jDown = Input.GetButtonDown("Jump");
        zDown = Input.GetButton("Fire1");
        gDown = Input.GetButtonDown("Fire2");
        rDown = Input.GetButtonDown("Reload");
        dDown = Input.GetButtonDown("Dodge");
        iDown = Input.GetButtonDown("Interaction");
        sDown1 = Input.GetButtonDown("Swap1");
        sDown2 = Input.GetButtonDown("Swap2");
        sDown3 = Input.GetButtonDown("Swap3");

    }
    //이동
    void Move()
    {
        //누른 방향으로 움직임
        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        moveVec = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        //회피중이면 운동방향은 회피방향이 됨
        if (isDodge)
        {
            moveVec = dodgeVec;
        }
        if (isSwap || !isAttackReady || isReload || isDead)
        {
            moveVec = Vector3.zero;
        }
        if (!isBorder)
            transform.position += moveVec * speed * (wDown ? 0.3f : 1f) * Time.deltaTime;
        //걷기


        anim.SetBool("isRun", moveVec != Vector3.zero);
        //anim.SetBool("isWalk", wDown);
    }
    /// <summary>
    //회전
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }
    /*점프
    void Jump()
    {
        if (jDown && !isJump && !isDead)
        {
            rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }*/
    //가까운 적 찾기
    void SearchEnemy()
    {
        
        Collider[] targetCollider = Physics.OverlapSphere(transform.position, targetRange, targetLayer); //지정한 위치로 부터 지정한 범위안에 접촉한 콜라이더 배열로 반환 (접촉 또는 범위안의 것들)
        Transform nearTarget = null;


        if (targetCollider.Length > 0) //가장 가까운 적 타겟팅
        {
            float nearDistance = Mathf.Infinity; //최소거리 초기화(무한대)
            foreach (Collider colTarget in targetCollider) //레이캐스트에 감지된 적들 중 가장 가까운 적 타겟팅
            {
                float targetDistance = Vector3.SqrMagnitude(transform.position - colTarget.transform.position); //타겟과의 거리 계산
                if (nearDistance > targetDistance) 
                {
                    nearDistance = targetDistance;
                    nearTarget = colTarget.transform;
                }
            }
        }
        targetTransform = nearTarget; //가장 가까운 적의 트랜스폼 저장
    }
    //공격
    void Attack() //공격
    {
        if (equipWeapon == null) {
            zDown = false;
            return;
        }
        AttackDelay += Time.deltaTime;
        isAttackReady = equipWeapon.AttackSpeed < AttackDelay; //장비한 무기속도

        if (zDown && !isDodge && isAttackReady && !isSwap && equipWeapon.curAmmo > 0 && !isShop && !isDead)
        {
            if (targetTransform != null) { 
                transform.LookAt(targetTransform.position); //공격시 SearchEnemy에서 검출한 가장 가까운 적 방향을 바라보도록 캐릭터 회전
            }
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            AttackDelay = 0;
            //zDown = false;
        }
        if (equipWeapon.curAmmo == 0 && equipWeapon.type==Weapon.Type.Range) //총알이 없을 시 공격상태를 false로 한다
            zDown = false;
    }
    
    void Grenade() //수류탄 투척
    {
        if (hasGrenades == 0)
        {
            gDown = false;
            return;
        }
        if (gDown && !isReload && !isSwap && !isShop && !isDead)
        {
            
            Vector3 grenadeVec = faceDirection.position - transform.position; //플레이어가 보는 방향

            GameObject instantGrenade = Instantiate(GrenadeObj, transform.position, transform.rotation);          
            Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
            rigidGrenade.AddForce(grenadeVec, ForceMode.Impulse);
            rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);

            hasGrenades--;
            //grenade[hasGrenades].SetActive(false);
            gDown = false;
            grenadeSound.Play();
        }
    }
    
    void Reload() //재장전
    {
        if (equipWeapon == null) 
        { 
            rDown = false;
            return;
        }
        if (equipWeapon.type == Weapon.Type.Melee) 
        {
            rDown = false;
            return;
        }
        if (ammo == 0)
        {
            rDown = false;
            return;
        }
        if (equipWeapon != null)
        {
            if (equipWeapon.curAmmo == equipWeapon.maxAmmo)
            {
                rDown = false;
                return;
            }
        }

            if (rDown && !isJump && !isDodge && !isSwap && isAttackReady && !isShop && !isDead)
        {
            
            anim.SetTrigger("doReload");
            isReload = true;
            rDown = false;
            reloadSound.Play();
            Invoke("ReloadOut", equipWeapon.reloadTime);
            
        }

    }

    void ReloadOut()
    {
        int remainAmmo;
        remainAmmo = equipWeapon.curAmmo;
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        equipWeapon.curAmmo = reAmmo;
        ammo = ammo-reAmmo+remainAmmo;
        isReload = false;
        
        
    }
    //회피
    void Dodge()
    {
        if (dDown && !isJump && !isDodge && !isShop && !isDead)
        {
            gameObject.layer = 14; //회피시 플레이어의 레이어를 무적(Invulnerable)레이어로 변경
            dodgeVec = moveVec; //회피방향=이동방향
            speed *= 2; //이동속도*2
            //rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            anim.SetTrigger("doDodge");
            isDodge = true;
            isDamaged = true;
            dDown = false;
            dodgeSound.Play();
            Invoke("DodgeOut", 0.5f); //0.5초후 회피종료 메소드 실행

        }
        else
            dDown = false;
    }
    //회피종료
    void DodgeOut()
    {
        gameObject.layer = 7; //Player 레이어로 복귀
        speed *= 0.5f; //이동속도 복귀
        isDodge = false;
        isDamaged = false;
    }
    //무기 교체
    void Swap() //무기 교체
    {
        if (sDown1 && (!hasWeapon[0] || equipWeaponIndex == 0))
        {
            sDown1 = false;
            sDown2 = false;
            sDown3 = false;
            return;
        }
        if (sDown2 && (!hasWeapon[1] || equipWeaponIndex == 1))
        {
            sDown1 = false;
            sDown2 = false;
            sDown3 = false;
            return;
        }
        if (sDown3 && (!hasWeapon[2] || equipWeaponIndex == 2))
        {
            sDown1 = false;
            sDown2 = false;
            sDown3 = false;
            return;
        }
        int weaponIndex = -1;
        if (sDown1)
            weaponIndex = 0;
        if (sDown2)
            weaponIndex = 1;
        if (sDown3)
            weaponIndex = 2;
        if ((sDown1 || sDown2 || sDown3) && !isJump && !isDodge && !isShop && !isDead)
        {
            if (equipWeapon != null)
                equipWeapon.gameObject.SetActive(false);

            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            weapons[weaponIndex].SetActive(true);

            anim.SetTrigger("doSwap");

            isSwap = true;
            sDown1 = false;
            sDown2 = false;
            sDown3 = false;
            swapSound.Play();

            Invoke("SwapOut", 0.5f);

        }
    }

    void SwapOut()
    {
        isSwap = false;
        
    }
    //아이템 상호작용
    void Interaction()
    {
        if (iDown && nearObject != null && !isDodge && !isJump && !isShop && !isDead)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.index;
                hasWeapon[weaponIndex] = true;
                iDown = false;
                iButton.SetActive(false);
                Destroy(nearObject);
                interSound.Play();
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
                iDown = false;
                interSound.Play();


            }
        }
    }


    //바닥에 닿았는지 검사
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }

    //무기나 상점콜라이더에 닿을때
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop") { 
            nearObject = other.gameObject;
            iButton.SetActive(true); //상호작용키 활성화
        }

    }
    //무기나 상점콜라이더에서 벗어났을때
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon") { 
            nearObject = null;
            iButton.SetActive(false);
        }
        else if (other.tag == "Shop")
        {
            //Shop shop = nearObject.GetComponent<Shop>();
            //shop.Exit();
            isShop = false;
            nearObject = null;
            iButton.SetActive(false); //상호작용키 비활성화
        }
    }
    //아이템콜라이더에 닿을때
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if (ammo > Maxammo)
                        ammo = Maxammo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin > Maxcoin)
                        coin = Maxcoin;
                    break;
                case Item.Type.Grenade:
                    
                    if (hasGrenades == MaxhasGrenades)
                        return;
                    hasGrenades += item.value;
                    if (hasGrenades > MaxhasGrenades)
                        hasGrenades = MaxhasGrenades;
                    //grenade[hasGrenades].SetActive(true);
                    
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health > Maxhealth)
                        health = Maxhealth;
                    break;

            }
            coinSound.Play();
            Destroy(other.gameObject);

        }
        else if (other.tag == "EnemyBullet") //피격시
        {
            ExplosionDamage enemyBullet = other.GetComponent<ExplosionDamage>();
            if (!isDamaged)
            {
                health -= enemyBullet.damage;
                hitVec = transform.position - other.transform.position;
                Vector3 hitVec2 = hitVec.normalized;
                float hitVecX = hitVec2.x;
                float hitVecZ = hitVec2.z;
                int hitback = enemyBullet.KnockbackForce;
                float hitbackX = hitVecX * hitback;
                float hitbackZ = hitVecZ * hitback;
                
                bool isBossAtk = other.name == "BossMeleeArea";
                rigid.AddForce(hitbackX,0,hitbackZ, ForceMode.Impulse); //피격시 적 공격의 넉백수치만큼 플레이어 넉백

                StartCoroutine(OnDamage(isBossAtk));
            }
        }
    }
    IEnumerator OnDamage(bool isBossAtk) //피격시
    {
        isDamaged = true;
        damagedSound.Play();
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.gray; //피격시 바디 색깔 회색
        }
        if (health <= 0 && !isDead) //체력이 0이하일 때
            OnDie();
        
        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
       
            
        yield return new WaitForSeconds(invulnerableTime);
        rigid.velocity = Vector3.zero;

        isBossAtk = false;
        isDamaged = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white; //바디 색깔 복귀
        }


    }
    //사망시
    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    //각 키를 누를 때마다 플레이어 상태 변화
    public void Dodgedown()
    {
        dDown = true;
    }
    public void Attackdown()
    {
        zDown = true;
    }
    public void AttackUp()
    {
        zDown = false;
    }
    public void Grenadedown()
    {
        gDown = true;
    }
    public void Reloaddown()
    {
        rDown = true;
    }
    public void Interdown()
    {
        iDown = true;
    }
    public void Swapdown1()
    {
        sDown1 = true;
    }
    public void Swapdown2()
    {
        sDown2 = true;

    }
    public void Swapdown3()
    {
        sDown3 = true;
    }
}
