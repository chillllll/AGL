using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    public float speed;
    public VariableJoystick variableJoystick;
    public GameObject iButton;
    public float jumpHeight;
    public GameObject[] weapons;
    public bool[] hasWeapon;
    public GameObject[] grenade;
    public int hasGrenades;
    public GameObject GrenadeObj;
    public int sateliteQuantity;
    public Transform faceDirection;
    public GameManager manager;

    public int ammo;
    public int coin;
    public int health;
    public int score;

    public int Maxammo;
    public int Maxcoin;
    public int Maxhealth;
    public int MaxhasGrenades;

    public float invulnerableTime;

    float hAxis;
    float vAxis;

    Vector3 moveVec;
    Vector3 dodgeVec;
    public bool wDown;
    public bool jDown;  
    public bool dDown;
    public AudioSource dodgeSound;
    public bool iDown;
    public AudioSource interSound;
    public bool zDown;
    public bool rDown;
    public AudioSource reloadSound;
    public bool gDown;
    public AudioSource grenadeSound;
    public bool sDown1;
    public bool sDown2;
    public bool sDown3;
    public AudioSource swapSound;
    public AudioSource damagedSound;
    public AudioSource coinSound;

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
    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }
    private void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void Update()
    {
        //getInput();
        Move();
        Turn();
        Jump();
        Dodge();
        Interaction();
        Swap();
        Attack();
        Reload();
        Grenade();

    }

    //인풋받기
    void getInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButtonDown("Jump");
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
        anim.SetBool("isWalk", wDown);
    }
    /// <summary>
    //회전
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }
    //점프
    void Jump()
    {
        if (jDown && !isJump && !isDead)
        {
            rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }
    }
    //가까운 적 찾기
    void SearchEnemy()
    {
        Collider[] targetCollider = Physics.OverlapSphere(transform.position, targetRange, targetLayer);
        Transform nearTarget = null;

        if (targetCollider.Length > 0)
        {
            float nearDistance = Mathf.Infinity; //최소거리 초기화(무한대)
            foreach (Collider colTarget in targetCollider)
            {
                float targetDistance = Vector3.SqrMagnitude(transform.position - colTarget.transform.position); //타겟과의 거리 계산
                if (nearDistance > targetDistance)
                {
                    nearDistance = targetDistance;
                    nearTarget = colTarget.transform;
                }
            }
        }
        targetTransform = nearTarget;
    }
    //공격
    void Attack()
    {
        if (equipWeapon == null) {
            zDown = false;
            return;
        }
        AttackDelay += Time.deltaTime;
        isAttackReady = equipWeapon.AttackSpeed < AttackDelay;

        if (zDown && !isDodge && isAttackReady && !isSwap && equipWeapon.curAmmo > 0 && !isShop && !isDead)
        {
            if (targetTransform != null) { 
                transform.LookAt(targetTransform.position);
            }
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            AttackDelay = 0;
            //zDown = false;
        }
        if (equipWeapon.curAmmo == 0 && equipWeapon.type==Weapon.Type.Range) 
            zDown = false;
    }
    
    void Grenade()
    {
        if (hasGrenades == 0)
        {
            gDown = false;
            return;
        }
        if (gDown && !isReload && !isSwap && !isShop && !isDead)
        {
            
            Vector3 grenadeVec = faceDirection.position - transform.position;

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
    void Reload()
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
            gameObject.layer = 14;
            dodgeVec = moveVec;
            speed *= 2;
            //rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            anim.SetTrigger("doDodge");
            isDodge = true;
            isDamaged = true;
            dDown = false;
            dodgeSound.Play();
            Invoke("DodgeOut", 0.5f);

        }
        else
            dDown = false;
    }
    //회피종료
    void DodgeOut()
    {
        gameObject.layer = 7;
        speed *= 0.5f;
        isDodge = false;
        isDamaged = false;
    }
    //무기 교체
    void Swap()
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

    //무기에 닿을때
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop") { 
            nearObject = other.gameObject;
            iButton.SetActive(true);
        }

    }

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
            iButton.SetActive(false);
        }
    }
    //아이템에 닿을때
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
                    if (hasGrenades > MaxhasGrenades)
                        hasGrenades = MaxhasGrenades;
                    if (hasGrenades == MaxhasGrenades)
                        return;
                    //grenade[hasGrenades].SetActive(true);
                    
                    hasGrenades += item.value;
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
        else if (other.tag == "EnemyBullet")
        {
            ExplosionDamage enemyBullet = other.GetComponent<ExplosionDamage>();
            if (!isDamaged)
            {
                health -= enemyBullet.damage;
                Vector3 hitVec = transform.position - other.transform.position;
                hitVec = hitVec.normalized;

                bool isBossAtk = other.name == "BossMeleeArea";

                rigid.AddForce(hitVec * enemyBullet.KnockbackForce, ForceMode.Impulse);
                StartCoroutine(OnDamage(isBossAtk));
            }
        }
    }
    IEnumerator OnDamage(bool isBossAtk)
    {
        isDamaged = true;
        damagedSound.Play();
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.gray;
        }
        if (health <= 0 && !isDead)
            OnDie();
        yield return new WaitForSeconds(invulnerableTime);
        if (isBossAtk)
            rigid.velocity = Vector3.zero;
        isBossAtk = false;
        isDamaged = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }


    }

    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }


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
