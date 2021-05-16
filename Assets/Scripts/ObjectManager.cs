using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    public GameObject bossBulletPrefab;
    public GameObject bossRockPrefab;

    public GameObject handgunBulletPrefab;
    public GameObject smgBulletPrefab;
    public GameObject grenadePrefab;

    public GameObject itemCoin1Prefab;
    public GameObject itemCoin2Prefab;
    public GameObject itemCoin3Prefab;
    public GameObject itemGrenadePrefab;
    public GameObject itemHealPrefab;
    public GameObject itemAmmoPrefab;

    GameObject[] enemyBullet; //몬스터 미사일
    GameObject[] bossBullet; //보스 미사일
    GameObject[] bossRock; //보스 바위

    GameObject[] handgunBullet; //플레이어 권총
    GameObject[] smgBullet; //플레이어 슴지
    GameObject[] grenade; // 플레이어 수류탄

    GameObject[] itemCoin1; //동전
    GameObject[] itemCoin2;
    GameObject[] itemCoin3;
    GameObject[] itemGrenade; //수류탄
    GameObject[] itemHeal; //힐
    GameObject[] itemAmmo; //총알

    GameObject[] targetPool;

    public int EbulletPool;
    public int BbulletPool;
    public int rockPool;
    public int HbulletPool;
    public int SbulletPool;
    public int grenadePool;
    public int coin1Pool;
    public int coin2Pool;
    public int coin3Pool;
    public int ammoPool;
    public int healPool;
    public int itemGPool;

    private void Awake()
    {
        enemyBullet = new GameObject[EbulletPool];
        bossBullet = new GameObject[BbulletPool];
        bossRock = new GameObject[rockPool];

        handgunBullet = new GameObject[HbulletPool];
        smgBullet = new GameObject[SbulletPool];
        grenade = new GameObject[grenadePool];

        itemCoin1 = new GameObject[coin1Pool];
        itemCoin2 = new GameObject[coin2Pool];
        itemCoin3 = new GameObject[coin3Pool];
        itemAmmo = new GameObject[ammoPool];
        itemGrenade = new GameObject[itemGPool];
        itemHeal = new GameObject[healPool];

        Generate();
    }

    void Generate() //풀 갯수만큼 생성
    {
        //1.enemy
        for (int index = 0; index < enemyBullet.Length; index++)
        {
            enemyBullet[index] = Instantiate(enemyBulletPrefab);
            enemyBullet[index].SetActive(false);
        }

        for (int index = 0; index < bossBullet.Length; index++)
        {
            bossBullet[index] = Instantiate(bossBulletPrefab);
            bossBullet[index].SetActive(false);
        }
        for (int index = 0; index < bossRock.Length; index++)
        {
            bossRock[index] = Instantiate(bossRockPrefab);
            bossRock[index].SetActive(false);
        }
        //2.player
        for (int index = 0; index < handgunBullet.Length; index++)
        {
            handgunBullet[index] = Instantiate(handgunBulletPrefab);
            handgunBullet[index].SetActive(false);
        }
        for (int index = 0; index < smgBullet.Length; index++)
        {
            smgBullet[index] = Instantiate(smgBulletPrefab);
            smgBullet[index].SetActive(false);
        }
        for (int index = 0; index < grenade.Length; index++)
        {
            grenade[index] = Instantiate(grenadePrefab);
            grenade[index].SetActive(false);
        }
        //3.Item
        for (int index = 0; index < itemCoin1.Length; index++)
        {
            itemCoin1[index] = Instantiate(itemCoin1Prefab);
            itemCoin1[index].SetActive(false);
        }
        for (int index = 0; index < itemCoin2.Length; index++)
        {
            itemCoin2[index] = Instantiate(itemCoin2Prefab);
            itemCoin2[index].SetActive(false);
        }
        for (int index = 0; index < itemCoin3.Length; index++)
        {
            itemCoin3[index] = Instantiate(itemCoin3Prefab);
            itemCoin3[index].SetActive(false);
        }
        for (int index = 0; index < itemAmmo.Length; index++)
        {
            itemAmmo[index] = Instantiate(itemAmmoPrefab);
            itemAmmo[index].SetActive(false);
        }
        for (int index = 0; index < itemGrenade.Length; index++)
        {
            itemGrenade[index] = Instantiate(itemGrenadePrefab);
            itemGrenade[index].SetActive(false);
        }
        for (int index = 0; index < itemHeal.Length; index++)
        {
            itemHeal[index] = Instantiate(itemHealPrefab);
            itemHeal[index].SetActive(false);
        }
    }

    public GameObject MakeObj(string type)
    {

        switch (type)
        {
            case " enemyBullet":
                targetPool = enemyBullet;
                break;
            case "bossBullet":
                targetPool = bossBullet;
                break;
            case "bossRock":
                targetPool = bossRock;
                break;

            case "handgunBullet":
                targetPool = handgunBullet;
                break;
            case "smgBullet":
                targetPool = smgBullet;
                break;
            case "greande":
                targetPool = grenade;
                break;
            case "itemCoin1":
                targetPool = itemCoin1;
                break;
            case "itemCoin2":
                targetPool = itemCoin2;
                break;
            case "itemCoin3":
                targetPool = itemCoin3;
                break;
            case "itemAmmo":
                targetPool = itemAmmo;
                break;
            case "itemHeal":
                targetPool = itemHeal;
                break;
            case "itemGrenade":
                targetPool = itemGrenade;
                break;

        }

        for (int index = 0; index < enemyBullet.Length; index++)
        {
            if (!enemyBullet[index].activeSelf) //활성화 되있지 않으면
            {
                enemyBullet[index].SetActive(true); //활성화
                return targetPool[index];
            }
        }

        return null;
    }
}
