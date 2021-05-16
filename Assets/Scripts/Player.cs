using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class Player : MonoBehaviour
{
    public float speed; //�̵��ӵ�
    public VariableJoystick variableJoystick;
    public GameObject iButton; //��ȣ�ۿ��ư
    public float jumpHeight; 
    public GameObject[] weapons; //�����
    public bool[] hasWeapon; //���� ���� ����
    //public GameObject[] grenade;
    public int hasGrenades; //���� ����ź ����
    public ObjectPooler grenadePool;
    public GameObject GrenadeObj;
    public int sateliteQuantity;
    public Transform faceDirection; //���� ����
    public GameManager manager; //���ӸŴ���

    public int ammo; //�Ѿ�
    public int coin; //����
    public int health; //ü��
    public int score; //����

    public int Maxammo; //�ִ��Ѿ˰���
    public int Maxcoin; //�ִ�����
    public int Maxhealth; //�ִ�ü��
    public int MaxhasGrenades; //�ִ����ź����

    public float invulnerableTime; //�����ð�

    float hAxis;
    float vAxis;

    Vector3 moveVec; //�̵�����
    Vector3 dodgeVec; //ȸ�ǹ���
    public bool wDown; //�̵�Ű����
    //public bool jDown;  
    public bool dDown; //ȸ��Ű����
    public AudioSource dodgeSound;
    public bool iDown; //��ȣ�ۿ�Ű����
    public AudioSource interSound;
    public bool zDown; //����Ű����
    public bool rDown; //����Ű����
    public AudioSource reloadSound;
    public bool gDown; //����źŰ����
    public AudioSource grenadeSound;
    public bool sDown1; //���⽺��Ű1����
    public bool sDown2; //���⽺��Ű2����
    public bool sDown3; //���⽺��Ű3����
    public AudioSource swapSound;
    public AudioSource damagedSound;
    public AudioSource coinSound;

    //�÷��̾� �ൿ ����
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

    //������� ����Ÿ������ ���� ����
    Transform shortTargetTrans; //�÷��̾�
    [Tooltip("Ÿ���� ����")]
    public float targetRange;
    [Tooltip("Ÿ�� ���̾�")]
    [SerializeField] LayerMask targetLayer;
    [Tooltip("Ÿ�� Ʈ������")]
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
    //ȸ������
    void FreezeRotation()
    {
        rigid.angularVelocity = Vector3.zero;
    }
    //������ ����� �ʰ� ����
    void StopToWall() //������ ������ʰ� ����
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isBorder = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall")); //���̸� ���� ���� ������ ����
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

    //��ǲ�ޱ�-����� ������
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
    //�̵�
    void Move()
    {
        //���� �������� ������
        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;
        moveVec = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        //ȸ�����̸� ������� ȸ�ǹ����� ��
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
        //�ȱ�


        anim.SetBool("isRun", moveVec != Vector3.zero);
        //anim.SetBool("isWalk", wDown);
    }
    /// <summary>
    //ȸ��
    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }
    /*����
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
    //����� �� ã��
    void SearchEnemy()
    {
        
        Collider[] targetCollider = Physics.OverlapSphere(transform.position, targetRange, targetLayer); //������ ��ġ�� ���� ������ �����ȿ� ������ �ݶ��̴� �迭�� ��ȯ (���� �Ǵ� �������� �͵�)
        Transform nearTarget = null;


        if (targetCollider.Length > 0) //���� ����� �� Ÿ����
        {
            float nearDistance = Mathf.Infinity; //�ּҰŸ� �ʱ�ȭ(���Ѵ�)
            foreach (Collider colTarget in targetCollider) //����ĳ��Ʈ�� ������ ���� �� ���� ����� �� Ÿ����
            {
                float targetDistance = Vector3.SqrMagnitude(transform.position - colTarget.transform.position); //Ÿ�ٰ��� �Ÿ� ���
                if (nearDistance > targetDistance) 
                {
                    nearDistance = targetDistance;
                    nearTarget = colTarget.transform;
                }
            }
        }
        targetTransform = nearTarget; //���� ����� ���� Ʈ������ ����
    }
    //����
    void Attack() //����
    {
        if (equipWeapon == null) {
            zDown = false;
            return;
        }
        AttackDelay += Time.deltaTime;
        isAttackReady = equipWeapon.AttackSpeed < AttackDelay; //����� ����ӵ�

        if (zDown && !isDodge && isAttackReady && !isSwap && equipWeapon.curAmmo > 0 && !isShop && !isDead)
        {
            if (targetTransform != null) { 
                transform.LookAt(targetTransform.position); //���ݽ� SearchEnemy���� ������ ���� ����� �� ������ �ٶ󺸵��� ĳ���� ȸ��
            }
            equipWeapon.Use();
            anim.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            AttackDelay = 0;
            //zDown = false;
        }
        if (equipWeapon.curAmmo == 0 && equipWeapon.type==Weapon.Type.Range) //�Ѿ��� ���� �� ���ݻ��¸� false�� �Ѵ�
            zDown = false;
    }
    
    void Grenade() //����ź ��ô
    {
        if (hasGrenades == 0)
        {
            gDown = false;
            return;
        }
        if (gDown && !isReload && !isSwap && !isShop && !isDead)
        {
            
            Vector3 grenadeVec = faceDirection.position - transform.position; //�÷��̾ ���� ����

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
    
    void Reload() //������
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
    //ȸ��
    void Dodge()
    {
        if (dDown && !isJump && !isDodge && !isShop && !isDead)
        {
            gameObject.layer = 14; //ȸ�ǽ� �÷��̾��� ���̾ ����(Invulnerable)���̾�� ����
            dodgeVec = moveVec; //ȸ�ǹ���=�̵�����
            speed *= 2; //�̵��ӵ�*2
            //rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
            anim.SetTrigger("doDodge");
            isDodge = true;
            isDamaged = true;
            dDown = false;
            dodgeSound.Play();
            Invoke("DodgeOut", 0.5f); //0.5���� ȸ������ �޼ҵ� ����

        }
        else
            dDown = false;
    }
    //ȸ������
    void DodgeOut()
    {
        gameObject.layer = 7; //Player ���̾�� ����
        speed *= 0.5f; //�̵��ӵ� ����
        isDodge = false;
        isDamaged = false;
    }
    //���� ��ü
    void Swap() //���� ��ü
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
    //������ ��ȣ�ۿ�
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


    //�ٴڿ� ��Ҵ��� �˻�
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "floor")
        {
            isJump = false;
            anim.SetBool("isJump", false);
        }
    }

    //���⳪ �����ݶ��̴��� ������
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop") { 
            nearObject = other.gameObject;
            iButton.SetActive(true); //��ȣ�ۿ�Ű Ȱ��ȭ
        }

    }
    //���⳪ �����ݶ��̴����� �������
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
            iButton.SetActive(false); //��ȣ�ۿ�Ű ��Ȱ��ȭ
        }
    }
    //�������ݶ��̴��� ������
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
        else if (other.tag == "EnemyBullet") //�ǰݽ�
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
                rigid.AddForce(hitbackX,0,hitbackZ, ForceMode.Impulse); //�ǰݽ� �� ������ �˹��ġ��ŭ �÷��̾� �˹�

                StartCoroutine(OnDamage(isBossAtk));
            }
        }
    }
    IEnumerator OnDamage(bool isBossAtk) //�ǰݽ�
    {
        isDamaged = true;
        damagedSound.Play();
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.gray; //�ǰݽ� �ٵ� ���� ȸ��
        }
        if (health <= 0 && !isDead) //ü���� 0������ ��
            OnDie();
        
        if (isBossAtk)
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
       
            
        yield return new WaitForSeconds(invulnerableTime);
        rigid.velocity = Vector3.zero;

        isBossAtk = false;
        isDamaged = false;
        foreach (MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white; //�ٵ� ���� ����
        }


    }
    //�����
    void OnDie()
    {
        anim.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }

    //�� Ű�� ���� ������ �÷��̾� ���� ��ȭ
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
