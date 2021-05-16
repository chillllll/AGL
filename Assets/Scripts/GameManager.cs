using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public GameObject gameCam; //메인카메라
    public Player player;
    public Boss boss;
    public int stage;
    public float playTime;
    public bool isBattle;
    //적의 개체수
    public int enemyCntA;
    public int enemyCntB;
    public int enemyCntC;
    public int enemyCntD;
    public GameObject clearZone;
    public GameObject GameOverSplash; //게임오버 ui
    public GameObject GameClearSplash; //게임클리어 ui
    //점수및 시간
    public Text curScoreTxt;
    public Text bestScoreTxt;
    public Text curTimeTxt;
    public Text bestTimeTxt;

    public Text MaxscoreTxt;
    public Text scoreTxt;
    public Text stageTxt;
    //플레이어 스테이터스
    public Text playTimeTxt;
    public Text playerHealth;
    public Text playerAmmo;
    public Text playerCoin;
    public Text playerGre;

    public GameObject weaponIcon1;
    public GameObject weaponIcon2;
    public GameObject weaponIcon3;
    public Image grenadeIcon;

    public Text enemyCntATxt;
    public Text enemyCntBTxt;
    public Text enemyCntCTxt;
    public RectTransform bossHealthGruop;
    public RectTransform bossHealthBar;

    public int targetFrame;
    void Awake()
    {
        //enemyList = new List<int>();
        //MaxscoreTxt.text = string.Format("{0,n0}", PlayerPrefs.GetInt("MaxScore"));
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrame;

    }

    void Update()
    {
        if (isBattle)
            playTime += Time.deltaTime;
        if (enemyCntA + enemyCntB + enemyCntC + enemyCntD > 0)
            return;
       // else
            //clearZone.SetActive(true);

    }

    void LateUpdate()
    {
        scoreTxt.text = string.Format("{0:n0}", player.score); //string.Format("{0:n0})", player.score); 숫자에 천단위로 ,찍음
        stageTxt.text = "STAGE " + stage;

        //플레이 시간
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        playTimeTxt.text = string.Format("{0:n0}", hour) + ":" + string.Format("{0:n0}", min) + ":" + string.Format("{0:n0}", second);
        playerHealth.text = player.health + " / " + player.Maxhealth;
        playerCoin.text = string.Format("{0:n0}", player.coin);
        playerGre.text = player.hasGrenades + " / " + player.MaxhasGrenades;
        if (player.equipWeapon == null)
            playerAmmo.text = "- / " + player.ammo;
        else if (player.equipWeapon.type == Weapon.Type.Melee)
            playerAmmo.text = "- / " + player.ammo;
        else
            playerAmmo.text = player.equipWeapon.curAmmo + " / " + player.ammo;
        
        //무기 소지시 무기 스왑 아이콘 활성화
        if (player.hasWeapon[0])
            weaponIcon1.SetActive(true);
        if (player.hasWeapon[1])
            weaponIcon2.SetActive(true);
        if (player.hasWeapon[2])
            weaponIcon3.SetActive(true);
        grenadeIcon.color = new Color(1, 1, 1, player.hasGrenades > 0 ? 1 : 0);

        enemyCntATxt.text = enemyCntA.ToString();
        enemyCntBTxt.text = enemyCntB.ToString();
        enemyCntCTxt.text = enemyCntC.ToString();

        bossHealthBar.localScale = new Vector3((float)boss.curHealth / boss.maxHealth, 1, 1);
    }


    public void StageStart()
    {
        isBattle = true;
    }
    public void StageEnd()
    {
        isBattle = false;
        StageClear();
    }

    public void GameOver()
    {
        GameOverSplash.SetActive(true);




    }

    public void StageClear()
    {
        GameClearSplash.SetActive(true);
        //클리어점수 표시
        curScoreTxt.text = scoreTxt.text;
        int maxScore = PlayerPrefs.GetInt("MaxScore");
        if (player.score > maxScore) //현 점수가 최고점수면 최고점수 갱신
        {
            bestScoreTxt.gameObject.SetActive(true);
            PlayerPrefs.SetInt("MaxScore", player.score);
        }
        //클리어시간 표시
        int hour = (int)(playTime / 3600);
        int min = (int)((playTime - hour * 3600) / 60);
        int second = (int)(playTime % 60);
        curTimeTxt.text = string.Format("{0:n0}", hour) + ":" + string.Format("{0:n0}", min) + ":" + string.Format("{0:n0}", second);
        int bestPlayTime = PlayerPrefs.GetInt("BestPlayTime");
        if (playTime < bestPlayTime) //현 시간이 최소시간이면 최소시간 갱신
        {
            bestScoreTxt.gameObject.SetActive(true);
            PlayerPrefs.SetFloat("BestPlayTime", playTime);
        }
    }


    IEnumerator InBattle()
    {

        yield return null;
    }
}
