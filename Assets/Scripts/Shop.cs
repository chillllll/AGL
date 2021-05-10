using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class Shop : MonoBehaviour
{

   // public RectTransform uiGroup;
    public Animator anim;
   // public UnityEvent shopEvent;
    public GameObject shopui;
    public GameObject[] itemObj;
    public int[] itemPrice;
    public Transform[] itemPosition;
    public Text talkText;
    public string[] talks;
    Player enterPlayer;
   


    public void Enter(Player player) //�÷��̾ ��ȣ�ۿ��
    {
        enterPlayer = player;
        //uiGroup.anchoredPosition = Vector3.zero;
        shopui.SetActive (true); //����ui ����
    }

    public void Exit()
    {

        anim.SetTrigger("doHelolo");
        shopui.SetActive(false);

    }

    public void Buy(int index) //������ ���Խ�
    {
        int price = itemPrice[index];
        if(price > enterPlayer.coin)
        {
            StopCoroutine(Talk());
            StartCoroutine(Talk());
            return;
        }

        enterPlayer.coin -= price; //���ݸ�ŭ ���� ����
        Vector3 ranvec = Vector3.right * Random.Range(-3, 3) + Vector3.forward * Random.Range(-3, 3); 
        Instantiate(itemObj[index], itemPosition[index].position + ranvec, itemPosition[index].rotation);  //�ֺ��� ������ ����

    }

    IEnumerator Talk()
    {
        talkText.text = talks[1];
        yield return new WaitForSeconds(2f);
        talkText.text = talks[0];
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
